using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace StdNounou.Stats.Core
{
    public abstract class SO_BaseStats_Core<StatsKey> : ScriptableObject, IStatContainer_Core<StatsKey>
    {
        [field: SerializeField] public string EntityName { get; private set; }
        [field: SerializeField] public SO_Affiliation<StatsKey> Affiliation { get; private set; }

        public bool TryGetStatData(StatsKey type, out StatData statData)
        {
            statData = null;
            if (GetStatsContainer().TryGetValue(type, out statData)) return true;
            return false;
        }

        public bool TryGetLowestAllowedValue(StatsKey type, out float value)
        {
            value = -1;
            if (GetStatsContainer().TryGetValue(type, out StatData statData))
            {
                value = statData.LowestAllowedValue;
                return true;
            }
            return false;
        }

        public bool TryGetStatValue(StatsKey type, out float value)
        {
            value = -1;
            if (GetStatsContainer().TryGetValue(type, out StatData statData))
            {
                value = statData.Value;
                return true;
            }
            return false;
        }

        public bool TryGetHigherAllowedValue(StatsKey type, out float value)
        {
            value = -1;
            if (GetStatsContainer().TryGetValue(type, out StatData statData))
            {
                value = statData.HigherAllowedValue;
                return true;
            }
            return false;
        }

        public abstract SerializedDictionary<StatsKey, StatData> GetStatsContainer();

        public override string ToString()
        => EntityName;
    } 
}