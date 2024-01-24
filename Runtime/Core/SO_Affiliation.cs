using AYellowpaper.SerializedCollections;
using StdNounou.Core;
using UnityEngine;

namespace StdNounou.Stats
{
    [CreateAssetMenu(fileName = "New Affiliation Data", menuName = "StdNounou/Scriptables/Stats/Affiliation Data")]
	public class SO_Affiliation : ScriptableObject
    {
		[field: SerializeField] public string ID { get; private set; }
		[field: SerializeField] public SerializedDictionary<SO_Affiliation, S_AffiliationModifiers> Modifiers { get; private set; }

		[System.Serializable]
		public struct S_AffiliationModifiers
		{
			[field: SerializeField] public bool AllowInteractions { get; private set; }
			[field: SerializeField] public SerializedDictionary<E_StatsKeys, S_StatModifiersContainer> StatsModificators { get; private set; }
		}

        [System.Serializable]
		public struct S_StatModifiersContainer
		{
            [field: SerializeField] public S_AffiliationStatModifier ReceivingModifiers { get; private set; }
            [field: SerializeField] public S_AffiliationStatModifier InflictingModifiers { get; private set; }
        }
        [System.Serializable]
		public struct S_AffiliationStatModifier
		{
			[field: SerializeField] public float Value { get; private set; }
			[field: SerializeField] public E_AffiliationStatModifierType ModificationType { get; private set; } 
		}

		public enum E_AffiliationStatModifierType
		{
			Additive,
			Multiplicative,
			Substractive,
			Subdivide
		}

		public bool TryGetAffiliationModifier(SO_Affiliation target, out S_AffiliationModifiers modifiers)
		{
			return Modifiers.TryGetValue(target, out modifiers);
		}
		public virtual float TryGetModifiedStat(SO_Affiliation target, E_StatsKeys statType, float statValue, bool receiver)
		{
            if (!TryGetAffiliationModifier(target, out var modifiers)) return statValue;
            if (!modifiers.AllowInteractions) return statValue;
            if (!modifiers.StatsModificators.TryGetValue(statType, out var modificator)) return statValue;
			return CalculateStatModification(statValue, modificator, receiver);
        }
        public bool AllowsInteractionsWith(SO_Affiliation target)
        {
            if (!TryGetAffiliationModifier(target, out var modifiers)) return true;
            return modifiers.AllowInteractions;
        }

        protected float CalculateStatModification(float statValue, S_StatModifiersContainer modifier, bool receiver)
		{
			S_AffiliationStatModifier modif = receiver ? modifier.ReceivingModifiers : modifier.InflictingModifiers;
			switch (modif.ModificationType)
			{
				case E_AffiliationStatModifierType.Additive:
					return statValue + modif.Value;

				case E_AffiliationStatModifierType.Multiplicative:
					return statValue * modif.Value;

				case E_AffiliationStatModifierType.Substractive:
					return statValue - modif.Value;

				case E_AffiliationStatModifierType.Subdivide:
					return modif.Value == 0 ? statValue : statValue / modif.Value;

				default:
					this.LogError($"No behavior for modifier type \"{modif.ModificationType}\" was defined.");
					return statValue;
			}
		}
	} 
}