using UnityEngine;

namespace StdNounou.Stats.Core
{
    public class SO_StatModifierData_Core<StatsKey> : ScriptableObject
    {
        [field: SerializeField] public string ID { get; private set; }

        [field: SerializeField] public bool Stackable { get; private set; }

        [field: SerializeField] public StatsKey StatType { get; private set; }
        [field: SerializeField] public float Amount { get; private set; }
        [field: SerializeField, 
            Tooltip("Additive : Adds the value to the stat.\n" +
                    "Multiplier : Multiply the base stat to the value and adds the result.\n" +
                    "Percentage : Gets the (value)% of base stat, and adds the result.")] public E_ModifierType ModifierType { get; private set; }

        [field: SerializeField] public bool Temporary { get; private set; }
        [field: SerializeField] public int TicksLifetime { get; private set; }

        public enum E_ModifierType
        {
            Additive,
            Multiplier,
        }
    }  
}