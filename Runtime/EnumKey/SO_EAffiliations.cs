using AYellowpaper.SerializedCollections;
using StdNounou.Stats.Core;
using UnityEngine;

namespace StdNounou.Stats
{
	[CreateAssetMenu(fileName = "New Affiliation Data", menuName = "StdNounou/Scriptables/Stats/Affiliation")]
	public class SO_EAffiliations : SO_Affiliation<E_StatTypes>
	{
		[field: SerializeField] public SerializedDictionary<SO_EAffiliations, S_AffiliationModifiers> AffiliationsModifiers { get; private set; }

        public override bool TryGetAffiliationModifier(SO_Affiliation<E_StatTypes> target, out S_AffiliationModifiers modifiers)
        {
            modifiers = default;
            SO_EAffiliations t = target as SO_EAffiliations;

            if (AffiliationsModifiers.ContainsKey(t))
            {
                modifiers = AffiliationsModifiers[t];
                return true;
            }
            return false;
        }
    } 
}