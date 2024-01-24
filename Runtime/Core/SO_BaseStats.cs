using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace StdNounou.Stats
{
    [CreateAssetMenu(fileName = "New BaseStats", menuName = "StdNounou/Scriptables/Stats/Base Stats")]
    public class SO_BaseStats : ScriptableObject, IStatContainer
    {
        [field: SerializeField] public string EntityName { get; private set; }
        [field: SerializeField] public SO_Affiliation Affiliation { get; private set; }
        [field: SerializeField] public SerializedDictionary<E_StatsKeys, StatData> Stats { get; private set; }

        public bool TryGetStatData(E_StatsKeys type, out StatData statData)
        {
            statData = null;
            if (Stats.TryGetValue(type, out statData)) return true;
            return false;
        }

        public bool TryGetLowestAllowedValue(E_StatsKeys type, out float value)
        {
            value = -1;
            if (Stats.TryGetValue(type, out StatData statData))
            {
                value = statData.LowestAllowedValue;
                return true;
            }
            return false;
        }

        public bool TryGetStatValue(E_StatsKeys type, out float value)
        {
            value = -1;
            if (Stats.TryGetValue(type, out StatData statData))
            {
                value = statData.Value;
                return true;
            }
            return false;
        }

        public bool TryGetHigherAllowedValue(E_StatsKeys type, out float value)
        {
            value = -1;
            if (Stats.TryGetValue(type, out StatData statData))
            {
                value = statData.HigherAllowedValue;
                return true;
            }
            return false;
        }

        public override string ToString()
        => EntityName;
    } 
}