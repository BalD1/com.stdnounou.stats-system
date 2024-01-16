using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace StdNounou.Stats.Core
{
	public abstract class SO_Affiliation<StatsKey> : ScriptableObject
	{
		[field: SerializeField] public string ID { get; private set; }

		[System.Serializable]
		public struct S_AffiliationModifiers
		{
			[field: SerializeField] public bool AllowInteractions { get; private set; }
			[field: SerializeField] public SerializedDictionary<StatsKey, float> StatsModificators { get; private set; }
		}

		public abstract bool TryGetAffiliationModifier(SO_Affiliation<StatsKey> target, out S_AffiliationModifiers modifiers);
	} 
}