using StdNounou.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StdNounou.Stats.Core
{
    [System.Serializable]
    public class StatsHandler_Core<StatsKey>
    {
        public StatsHandler_Core(SO_BaseStats_Core<StatsKey> baseStats)
        {
            this.BaseStats = baseStats;
        }

        [field: SerializeField] public SO_BaseStats_Core<StatsKey> BaseStats { get; private set; }
        public Dictionary<StatsKey, float> PermanentBonusStats { get; protected set; } = new Dictionary<StatsKey, float>();
        public Dictionary<StatsKey, float> TemporaryBonusStats { get; protected set; } = new Dictionary<StatsKey, float>();
        public Dictionary<StatsKey, float> BrutFinalStats { get; protected set; } = new Dictionary<StatsKey, float>();

        public Dictionary<string, StatsModifier_Core<StatsKey>> UniqueStatsModifiers { get; protected set; } = new Dictionary<string, StatsModifier_Core<StatsKey>>();
        public Dictionary<string, List<StatsModifier_Core<StatsKey>>> StackableStatsModifiers { get; protected set; } = new Dictionary<string, List<StatsModifier_Core<StatsKey>>>();

        public event Action OnAskReset;

        public bool debug;

        public enum E_ModifierAddResult
        {
            Success,
            StatAlreadyMaxed,
            Unstackable,
        }

        public event Action<StatChangeEventArgs<StatsKey>> OnStatChange;

        public void InitializeDictionaries()
        {
            PermanentBonusStats = new Dictionary<StatsKey, float>();
            TemporaryBonusStats = new Dictionary<StatsKey, float>();
            BrutFinalStats = new Dictionary<StatsKey, float>();

            UniqueStatsModifiers = new Dictionary<string, StatsModifier_Core<StatsKey>>();
            StackableStatsModifiers = new Dictionary<string, List<StatsModifier_Core<StatsKey>>>();
            if (BaseStats == null)
            {
                this.LogError("Stats SO was not set.");
                return;
            }
            foreach (var item in BaseStats.GetStatsContainer())
            {
                BrutFinalStats.Add(item.Key, item.Value.Value);
                PermanentBonusStats.Add(item.Key, 0);
                TemporaryBonusStats.Add(item.Key, 0);
            }

#if UNITY_EDITOR
            if (debug) this.Log("Successfully initialized dictionaries");
#endif
        }

        /// <summary>
        /// Tries to out "<paramref name="value"/>" the base stat of type "<paramref name="type"/>".
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetBaseStat(StatsKey type, out float value)
        {
            return BaseStats.TryGetStatValue(type, out value);
        }

        /// <summary>
        /// Tries to out "<paramref name="value"/>" the final stat, after all modifiers calculations, of type <paramref name="type"/>".
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetFinalStat(StatsKey type, out float value)
        {
            bool res = BrutFinalStats.TryGetValue(type, out value);
            if (!res) return false;

            float higherAllowedValue = BaseStats.GetStatsContainer()[type].HigherAllowedValue;
            float lowestAllowedValue = BaseStats.GetStatsContainer()[type].LowestAllowedValue;

            if (value > higherAllowedValue)
                value = higherAllowedValue;
            else if (value < lowestAllowedValue)
                value = lowestAllowedValue;

            return true;
        }

        public bool TryAddModifier(SO_StatModifierData_Core<StatsKey> data, out E_ModifierAddResult result)
        {
            if (data.Stackable)
                return TryAddStackableModifier(data, out result);
            else
                return TryAddUniqueModifier(data, out result);
        }

        private bool TryAddStackableModifier(SO_StatModifierData_Core<StatsKey> data, out E_ModifierAddResult result)
        {
#if UNITY_EDITOR
            if (debug) this.Log($"Try Add Stackable Modifier {data.ID}");
#endif
            if (!TrySetModifier(data))
            {
                result = E_ModifierAddResult.StatAlreadyMaxed;
#if UNITY_EDITOR
                if (debug) this.Log($"Failed to add modifier {data.ID} : Stats {data.StatType} already maxed.");
#endif
                return false;
            }

            //add the modifier to the list
            if (!StackableStatsModifiers.ContainsKey(data.ID))
                StackableStatsModifiers.Add(data.ID, new List<StatsModifier_Core<StatsKey>>());
            StackableStatsModifiers[data.ID].Add(new StatsModifier_Core<StatsKey>(data, this));

            result = E_ModifierAddResult.Success;
            ModifyBrutFinalStat(data.StatType, GetModifierValue(data));

#if UNITY_EDITOR
            if (debug) this.Log($"Successfuly added modifier {data.ID}.");
#endif
            return true;
        }

        private bool TryAddUniqueModifier(SO_StatModifierData_Core<StatsKey> data, out E_ModifierAddResult result)
        {
            // if the unique modifier already exists, return
            if (UniqueStatsModifiers.ContainsKey(data.ID))
            {
                result = E_ModifierAddResult.Unstackable;
#if UNITY_EDITOR
                if (debug) this.Log($"Failed to add modifier {data.ID} : target already contains an instance.");
#endif
                return false;
            }
            if (!TrySetModifier(data))
            {
                result = E_ModifierAddResult.StatAlreadyMaxed;
#if UNITY_EDITOR
                if (debug) this.Log($"Failed to add modifier {data.ID} : Stats {data.StatType} already maxed.");
#endif
                return false;
            }

            UniqueStatsModifiers.Add(data.ID, new StatsModifier_Core<StatsKey>(data, this));
            ModifyBrutFinalStat(data.StatType, GetModifierValue(data));
            result = E_ModifierAddResult.Success;
#if UNITY_EDITOR
            if (debug) this.Log($"Successfuly added modifier {data.ID}.");
#endif
            return true;
        }

        private bool TrySetModifier(SO_StatModifierData_Core<StatsKey> data)
        {
            float modifierValue = GetModifierValue(data);
            if (data.Temporary)
            {
                if (TemporaryBonusStats.ContainsKey(data.StatType))
                    TemporaryBonusStats[data.StatType] += modifierValue;
                else
                    TemporaryBonusStats.Add(data.StatType, modifierValue);
            }
            else
            {
                // else, check if the sum of all permanent bonus is >= of max, return.
                // else, add it to permanent bonuses
                if (!PermanentBonusStats.ContainsKey(data.StatType))
                {
#if UNITY_EDITOR
                    if (debug) this.LogError($"Stat \"{data.StatType}\" type was not present in dictionnary.");
#endif
                    return false;
                }
                else
                {
                    if (PermanentBonusStats[data.StatType] >= (BaseStats.GetStatsContainer()[data.StatType].HigherAllowedValue - BaseStats.GetStatsContainer()[data.StatType].Value))
                        return false;
                    if (PermanentBonusStats.ContainsKey(data.StatType))
                        PermanentBonusStats[data.StatType] += modifierValue;
                    else
                        PermanentBonusStats.Add(data.StatType, modifierValue);
                }
            }
            return true;
        }

        private float GetModifierValue(SO_StatModifierData_Core<StatsKey> data)
        {
            switch (data.ModifierType)
            {
                case SO_StatModifierData_Core<StatsKey>.E_ModifierType.Additive:
                    return data.Amount;
                case SO_StatModifierData_Core<StatsKey>.E_ModifierType.Multiplier:
                    BaseStats.TryGetStatValue(data.StatType, out float baseStatValue);
                    return baseStatValue * data.Amount;
            }
            return 0;
        }

        private void ModifyBrutFinalStat(StatsKey type, float value)
        {
            if (BrutFinalStats.ContainsKey(type))
            {
                BrutFinalStats[type] += value;
                OnStatChange?.Invoke(new StatChangeEventArgs<StatsKey>(type, value, BrutFinalStats[type]));
            }
        }

        public void RemoveStatModifier(StatsModifier_Core<StatsKey> modifier)
        {
#if UNITY_EDITOR
            if (debug) this.Log($"Removing modifier {modifier.Data.ID}");
#endif
            float modifierValue = GetModifierValue(modifier.Data);
            if (modifier.Data.Temporary)
                TemporaryBonusStats[modifier.Data.StatType] -= modifierValue;
            else
                PermanentBonusStats[modifier.Data.StatType] -= modifierValue;

            if (modifier.Data.Stackable)
            {
                ModifyBrutFinalStat(modifier.Data.StatType, -modifierValue);
                StackableStatsModifiers[modifier.Data.ID].Remove(modifier);

                return;
            }

            ModifyBrutFinalStat(modifier.Data.StatType, -modifierValue);
            UniqueStatsModifiers.Remove(modifier.Data.ID);
        }

        public void ChangeBaseStats(SO_BaseStats_Core<StatsKey> stats, bool resetModifiers)
        {
            BaseStats = stats;
            if (resetModifiers) RemoveAllModifiers();
        }

        public void RemoveAllModifiers()
        {
            OnAskReset?.Invoke();
        }

        public bool TryGetAffiliationModifiersOf(SO_Affiliation<StatsKey> target, out SO_Affiliation<StatsKey>.S_AffiliationModifiers modifiers)
        {
            return BaseStats.Affiliation.TryGetAffiliationModifier(target, out modifiers);
        }

        public SO_Affiliation<StatsKey> GetAffiliation()
            => BaseStats.Affiliation;
    }

    public class StatChangeEventArgs<StatsKey> : EventArgs
    {
        public StatChangeEventArgs(StatsKey statKey, float modifier, float finalVal)
        {
            this.StatKey = statKey;
            this.ModifierValue = modifier;
            this.FinalValue = finalVal;
        }
        public StatsKey StatKey { get; private set; }
        public float ModifierValue { get; private set; }
        public float FinalValue { get; private set; }
    }
}