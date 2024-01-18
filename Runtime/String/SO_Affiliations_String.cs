using AYellowpaper.SerializedCollections;
using StdNounou.Stats.Core;
using UnityEngine;

namespace StdNounou.Stats
{
	[CreateAssetMenu(fileName = "New Affiliation Data", menuName = "StdNounou/Scriptables/Stats/Affiliation/String")]
	public class SO_Affiliations_String : SO_Affiliation<string>
	{
		[field: SerializeField] public SerializedDictionary<SO_Affiliations_String, S_AffiliationModifiers> AffiliationsModifiers { get; private set; }

        public override bool TryGetAffiliationModifier(SO_Affiliation<string> target, out S_AffiliationModifiers modifiers)
        {
            modifiers = default;
            SO_Affiliations_String t = target as SO_Affiliations_String;

            if (AffiliationsModifiers.ContainsKey(t))
            {
                modifiers = AffiliationsModifiers[t];
                return true;
            }
            return false;
        }
    } 
}