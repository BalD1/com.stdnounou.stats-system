using StdNounou.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StdNounou.Stats
{
    [System.Serializable]
    public class StatsHandler
    {
        public StatsHandler(SO_BaseStats baseStats)
        {
            this.BaseStats = baseStats;
        }

        [field: SerializeField] public SO_BaseStats BaseStats { get; private set; }
        public Dictionary<E_StatsKeys, float> PermanentBonusStats { get; protected set; } = new Dictionary<E_StatsKeys, float>();
        public Dictionary<E_StatsKeys, float> TemporaryBonusStats { get; protected set; } = new Dictionary<E_StatsKeys, float>();
        public Dictionary<E_StatsKeys, float> BrutFinalStats { get; protected set; } = new Dictionary<E_StatsKeys, float>();

        public Dictionary<string, StatsModifier> UniqueStatsModifiers { get; protected set; } = new Dictionary<string, StatsModifier>();
        public Dictionary<string, List<StatsModifier>> StackableStatsModifiers { get; protected set; } = new Dictionary<string, List<StatsModifier>>();

        public event Action OnAskReset;

        public bool debug;

        public enum E_ModifierAddResult
        {
            Success,
            StatAlreadyMaxed,
            Unstackable,
        }

        public event Action<StatChangeEventArgs> OnStatChange;

        public void InitializeDictionaries()
        {
            PermanentBonusStats = new Dictionary<E_StatsKeys, float>();
            TemporaryBonusStats = new Dictionary<E_StatsKeys, float>();
            BrutFinalStats = new Dictionary<E_StatsKeys, float>();

            UniqueStatsModifiers = new Dictionary<string, StatsModifier>();
            StackableStatsModifiers = new Dictionary<string, List<StatsModifier>>();
            if (BaseStats == null)
            {
                this.LogError("Stats SO was not set.");
                return;
            }
            foreach (var item in BaseStats.Stats)
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
        public bool TryGetBaseStat(E_StatsKeys type, out float value)
        {
            return BaseStats.TryGetStatValue(type, out value);
        }

        /// <summary>
        /// Tries to out "<paramref name="value"/>" the final stat, after all modifiers calculations, of type <paramref name="type"/>".
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetFinalStat(E_StatsKeys type, out float value)
        {
            bool res = BrutFinalStats.TryGetValue(type, out value);
            if (!res) return false;

            float higherAllowedValue = BaseStats.Stats[type].HigherAllowedValue;
            float lowestAllowedValue = BaseStats.Stats[type].LowestAllowedValue;

            if (value > higherAllowedValue)
                value = higherAllowedValue;
            else if (value < lowestAllowedValue)
                value = lowestAllowedValue;

            return true;
        }

        public bool TryAddModifier(SO_StatModifierData data, out E_ModifierAddResult result, out StatsModifier modifier)
        {
            if (data.Stackable)
                return TryAddStackableModifier(data, out result, out modifier);
            else
                return TryAddUniqueModifier(data, out result, out modifier);
        }

        private bool TryAddStackableModifier(SO_StatModifierData data, out E_ModifierAddResult result, out StatsModifier modifier)
        {
#if UNITY_EDITOR
            if (debug) this.Log($"Try Add Stackable Modifier {data.ID}");
#endif
            if (!TrySetModifier(data))
            {
                result = E_ModifierAddResult.StatAlreadyMaxed;
                modifier = null;
#if UNITY_EDITOR
                if (debug) this.Log($"Failed to add modifier {data.ID} : Stats {data.StatType} already maxed.");
#endif
                return false;
            }

            //add the modifier to the list
            if (!StackableStatsModifiers.ContainsKey(data.ID))
                StackableStatsModifiers.Add(data.ID, new List<StatsModifier>());
            modifier = new StatsModifier(data, this);
            StackableStatsModifiers[data.ID].Add(modifier);

            result = E_ModifierAddResult.Success;
            ModifyBrutFinalStat(data.StatType, GetModifierValue(data));

#if UNITY_EDITOR
            if (debug) this.Log($"Successfuly added modifier {data.ID}.");
#endif
            return true;
        }

        private bool TryAddUniqueModifier(SO_StatModifierData data, out E_ModifierAddResult result, out StatsModifier modifier)
        {
            // if the unique modifier already exists, return
            if (UniqueStatsModifiers.ContainsKey(data.ID))
            {
                result = E_ModifierAddResult.Unstackable;
                modifier = null;
#if UNITY_EDITOR
                if (debug) this.Log($"Failed to add modifier {data.ID} : target already contains an instance.");
#endif
                return false;
            }
            if (!TrySetModifier(data))
            {
                result = E_ModifierAddResult.StatAlreadyMaxed;
                modifier = null;
#if UNITY_EDITOR
                if (debug) this.Log($"Failed to add modifier {data.ID} : Stats {data.StatType} already maxed.");
#endif
                return false;
            }

            modifier = new StatsModifier(data, this);
            UniqueStatsModifiers.Add(data.ID, modifier);
            ModifyBrutFinalStat(data.StatType, GetModifierValue(data));
            result = E_ModifierAddResult.Success;
#if UNITY_EDITOR
            if (debug) this.Log($"Successfuly added modifier {data.ID}.");
#endif
            return true;
        }

        private bool TrySetModifier(SO_StatModifierData data)
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
                    if (PermanentBonusStats[data.StatType] >= (BaseStats.Stats[data.StatType].HigherAllowedValue - BaseStats.Stats[data.StatType].Value))
                        return false;
                    if (PermanentBonusStats.ContainsKey(data.StatType))
                        PermanentBonusStats[data.StatType] += modifierValue;
                    else
                        PermanentBonusStats.Add(data.StatType, modifierValue);
                }
            }
            return true;
        }

        private float GetModifierValue(SO_StatModifierData data)
        {
            switch (data.ModifierType)
            {
                case SO_StatModifierData.E_ModifierType.Additive:
                    return data.Amount;
                case SO_StatModifierData.E_ModifierType.Multiplier:
                    BaseStats.TryGetStatValue(data.StatType, out float baseStatValue);
                    return baseStatValue * data.Amount;
            }
            return 0;
        }

        private void ModifyBrutFinalStat(E_StatsKeys type, float value)
        {
            if (BrutFinalStats.ContainsKey(type))
            {
                BrutFinalStats[type] += value;
                OnStatChange?.Invoke(new StatChangeEventArgs(type, value, BrutFinalStats[type]));
            }
        }

        public void RemoveStatModifier(StatsModifier modifier)
        {
#if UNITY_EDITOR
            if (debug) this.Log($"Removing modifier {modifier.Data.ID}");
#endif
            float modifierValue = GetModifierValue(modifier.Data);
            if (modifier.Data.Stackable)
            {
                if (!StackableStatsModifiers.ContainsKey(modifier.Data.ID) ||
                    StackableStatsModifiers[modifier.Data.ID].Count == 0)
                {
                    this.LogError($"Could not remove {modifier} ({modifier.Data.ID}) : was not found in dictionnary.");
                    return;
                }
                ModifyBrutFinalStat(modifier.Data.StatType, -modifierValue);
                StackableStatsModifiers[modifier.Data.ID].Remove(modifier);
            }
            else
            {
                if (!UniqueStatsModifiers.ContainsKey(modifier.Data.ID))
                {
                    this.LogError($"Could not remove {modifier} ({modifier.Data.ID}) : was not found in dictionnary.");
                    return;
                }
                ModifyBrutFinalStat(modifier.Data.StatType, -modifierValue);
                UniqueStatsModifiers.Remove(modifier.Data.ID);
            }

            if (modifier.Data.Temporary)
                TemporaryBonusStats[modifier.Data.StatType] -= modifierValue;
            else
                PermanentBonusStats[modifier.Data.StatType] -= modifierValue;
        }

        public void ChangeBaseStats(SO_BaseStats stats, bool resetModifiers)
        {
            BaseStats = stats;
            if (resetModifiers) RemoveAllModifiers();
        }

        public void RemoveAllModifiers()
        {
            OnAskReset?.Invoke();
        }

        public bool TryGetAffiliationModifiersOf(SO_Affiliation target, out SO_Affiliation.S_AffiliationModifiers modifiers)
        {
            return BaseStats.Affiliation.TryGetAffiliationModifier(target, out modifiers);
        }

        public float TryGetModifiedStatFromAttribute(SO_Attribute target, E_StatsKeys stat, float statValue)
        {
            foreach (var item in BaseStats.Attributes)
            {
                statValue = item.TryGetModifiedStat(target, stat, statValue);
            }
            return statValue;
        }

        public bool AllowsInteractionsWith(SO_Affiliation target)
            => BaseStats.Affiliation.AllowsInteractionsWith(target);

        public SO_Affiliation GetAffiliation()
            => BaseStats.Affiliation;
    }

    public class StatChangeEventArgs : EventArgs
    {
        public StatChangeEventArgs(E_StatsKeys statKey, float modifier, float finalVal)
        {
            this.StatKey = statKey;
            this.ModifierValue = modifier;
            this.FinalValue = finalVal;
        }
        public E_StatsKeys StatKey { get; private set; }
        public float ModifierValue { get; private set; }
        public float FinalValue { get; private set; }
    }
}