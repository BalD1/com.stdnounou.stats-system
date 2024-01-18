using AYellowpaper.SerializedCollections;
using StdNounou.Core;
using StdNounou.Stats.Core;
using UnityEngine;

namespace StdNounou.Stats
{
	[CreateAssetMenu(fileName = "New Affiliation Data", menuName = "StdNounou/Scriptables/Stats/Affiliation/Key")]
    public class SO_Affiliations_Key : SO_Affiliation<SO_KeyContainer>
    {
        [field: SerializeField] public SerializedDictionary<SO_Affiliations_Key, S_AffiliationModifiers> AffiliationsModifiers { get; private set; }

        public override bool TryGetAffiliationModifier(SO_Affiliation<SO_KeyContainer> target, out S_AffiliationModifiers modifiers)
        {
            modifiers = default;
            SO_Affiliations_Key t = target as SO_Affiliations_Key;

            if (AffiliationsModifiers.ContainsKey(t))
            {
                modifiers = AffiliationsModifiers[t];
                return true;
            }
            return false;
        }
    } 
}