using AYellowpaper.SerializedCollections;
using StdNounou.Core;
using StdNounou.Stats.Core;
using UnityEngine;

namespace StdNounou.Stats
{
	[CreateAssetMenu(fileName = "New BaseStats", menuName = "StdNounou/Scriptables/Stats/BaseStats/Key")]
	public class SO_BaseStats_Key : SO_BaseStats_Core<SO_KeyContainer>
    {
        [SerializeField] private SerializedDictionary<SO_KeyContainer, StatData> stats;

        public override SerializedDictionary<SO_KeyContainer, StatData> GetStatsContainer()
            => stats;
    } 
}
