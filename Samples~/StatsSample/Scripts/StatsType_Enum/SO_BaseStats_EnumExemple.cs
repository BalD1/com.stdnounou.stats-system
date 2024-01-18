using AYellowpaper.SerializedCollections;
using StdNounou.Stats.Core;
using UnityEngine;

namespace StdNounou.Stats.Samples
{
	[CreateAssetMenu(fileName = "New BaseStats", menuName = "StdNounou/Scriptables/Stats/BaseStats/Enum")]
	public class SO_BaseStats_EnumExemple : SO_BaseStats_Core<E_StatsEnumExemple>
    {
        [SerializeField] private SerializedDictionary<E_StatsEnumExemple, StatData> stats;

        public override SerializedDictionary<E_StatsEnumExemple, StatData> GetStatsContainer()
            => stats;
    } 
}
