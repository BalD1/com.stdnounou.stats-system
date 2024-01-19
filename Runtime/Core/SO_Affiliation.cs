using AYellowpaper.SerializedCollections;
using StdNounou.Core;
using UnityEngine;

namespace StdNounou.Stats.Core
{
	public abstract class SO_Affiliation<StatsKey> : ScriptableObject
	{
		[field: SerializeField] public string ID { get; private set; }

		[System.Serializable]
		public struct S_AffiliationModifiers
		{
			[field: SerializeField] public bool AllowInteractions { get; private set; }
			[field: SerializeField] public SerializedDictionary<StatsKey, S_AffiliationStatModifier> StatsModificators { get; private set; }
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

		public abstract bool TryGetAffiliationModifier(SO_Affiliation<StatsKey> target, out S_AffiliationModifiers modifiers);
		public virtual float TryGetModifiedStat(SO_Affiliation<StatsKey> target, StatsKey statType, float statValue)
		{
            if (!TryGetAffiliationModifier(target, out var modifiers)) return statValue;
            if (!modifiers.AllowInteractions) return statValue;
            if (!modifiers.StatsModificators.TryGetValue(statType, out var modificator)) return statValue;
			return CalculateStatModification(statValue, modificator);
        }
        public bool AllowsInteractionsWith(SO_Affiliation<StatsKey> target)
        {
            if (!TryGetAffiliationModifier(target, out var modifiers)) return true;
            return modifiers.AllowInteractions;
        }

        protected float CalculateStatModification(float statValue, S_AffiliationStatModifier modifier)
		{
			switch (modifier.ModificationType)
			{
				case E_AffiliationStatModifierType.Additive:
					return statValue + modifier.Value;

				case E_AffiliationStatModifierType.Multiplicative:
					return statValue * modifier.Value;

				case E_AffiliationStatModifierType.Substractive:
					return statValue - modifier.Value;

				case E_AffiliationStatModifierType.Subdivide:
					return modifier.Value == 0 ? statValue : statValue / modifier.Value;

				default:
					this.LogError($"No behavior for modifier type \"{modifier.ModificationType}\" was defined.");
					return statValue;
			}
		}
	} 
}