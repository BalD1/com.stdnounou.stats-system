using AYellowpaper.SerializedCollections;
using StdNounou.Core;
using UnityEngine;

namespace StdNounou.Stats
{
	[CreateAssetMenu(fileName = "New Attribute Data", menuName = "StdNounou/Scriptables/Stats/Attribute Data", order = 13)]
	public class SO_Attribute : ScriptableObject
	{
		[field: SerializeField] public string ID {  get; private set; }

        [field: SerializeField, SerializedDictionary("Affiliations", "Modifiers")] 
        public SerializedDictionary<SO_Attribute, SerializedDictionary<E_StatsKeys, S_AttributeStatModifier>> Modifiers { get; private set; }

        [System.Serializable]
        public struct S_AttributeStatModifier
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

        public bool TryGetAttributeModifier(SO_Attribute target, out SerializedDictionary<E_StatsKeys, S_AttributeStatModifier> modifiers)
        {
            return Modifiers.TryGetValue(target, out modifiers);
        }
        public virtual float TryGetModifiedStat(SO_Attribute target, E_StatsKeys statType, float statValue)
        {
            if (!Modifiers.TryGetValue(target, out var modifiers)) return statValue;
            if (!modifiers.TryGetValue(statType, out var modificator)) return statValue;
            return CalculateStatModification(statValue, modificator);
        }

        protected float CalculateStatModification(float statValue, S_AttributeStatModifier modifier)
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