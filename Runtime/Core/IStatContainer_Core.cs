using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace StdNounou.Stats.Core
{
    public interface IStatContainer_Core<StatsKey>
    {
        public abstract bool TryGetStatData(StatsKey type, out StatData statData);
        public bool TryGetLowestAllowedValue(StatsKey type, out float value);
        public bool TryGetStatValue(StatsKey type, out float value);
        public bool TryGetHigherAllowedValue(StatsKey type, out float value);

        public SerializedDictionary<StatsKey, StatData> GetStatsContainer();
    }

    [System.Serializable]
    public class StatData
    {
        [field: SerializeField] public float LowestAllowedValue { get; private set; }
        [field: SerializeField] public float Value { get; private set; }
        [field: SerializeField] public float HigherAllowedValue { get; private set; }
    }
}