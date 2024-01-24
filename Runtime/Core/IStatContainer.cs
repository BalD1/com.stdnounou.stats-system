using UnityEngine;

namespace StdNounou.Stats
{
    public interface IStatContainer
    {
        public abstract bool TryGetStatData(E_StatsKeys type, out StatData statData);
        public bool TryGetLowestAllowedValue(E_StatsKeys type, out float value);
        public bool TryGetStatValue(E_StatsKeys type, out float value);
        public bool TryGetHigherAllowedValue(E_StatsKeys type, out float value);
    }

    [System.Serializable]
    public class StatData
    {
        [field: SerializeField] public float LowestAllowedValue { get; private set; }
        [field: SerializeField] public float Value { get; private set; }
        [field: SerializeField] public float HigherAllowedValue { get; private set; }
    }
}