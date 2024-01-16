using AYellowpaper.SerializedCollections;
using StdNounou.Stats.Core;
using UnityEngine;

namespace StdNounou.Stats
{
	[CreateAssetMenu(fileName = "New BaseStats", menuName = "StdNounou/Scriptables/Stats/BaseStats")]
	public class SO_EBaseStats : SO_BaseStats_Core<E_StatTypes>
	{
        [SerializeField] private SerializedDictionary<E_StatTypes, StatData> stats;

        public override SerializedDictionary<E_StatTypes, StatData> GetStatsContainer()
            => stats;
    }
}