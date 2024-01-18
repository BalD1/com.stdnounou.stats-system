using AYellowpaper.SerializedCollections;
using StdNounou.Stats.Core;
using UnityEngine;

namespace StdNounou.Stats
{
	[CreateAssetMenu(fileName = "New BaseStats", menuName = "StdNounou/Scriptables/Stats/BaseStats/String")]
	public class SO_BaseStats_String : SO_BaseStats_Core<string>
	{
        [SerializeField] private SerializedDictionary<string, StatData> stats;

        public override SerializedDictionary<string, StatData> GetStatsContainer()
            => stats;
    }
}