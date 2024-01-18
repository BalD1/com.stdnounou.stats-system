using AYellowpaper.SerializedCollections;
using StdNounou.Stats.Core;
using UnityEngine;

namespace StdNounou.Stats.Samples
{
	[CreateAssetMenu(fileName = "New Affiliation Data", menuName = "StdNounou/Scriptables/Stats/Affiliation/EnumExemple")]
    public class SO_Affiliations_EnumExemple : SO_Affiliation<E_StatsEnumExemple>
    {
        [field: SerializeField] public SerializedDictionary<SO_Affiliations_EnumExemple, S_AffiliationModifiers> AffiliationsModifiers { get; private set; }

        public override bool TryGetAffiliationModifier(SO_Affiliation<E_StatsEnumExemple> target, out S_AffiliationModifiers modifiers)
        {
            modifiers = default;
            SO_Affiliations_EnumExemple t = target as SO_Affiliations_EnumExemple;

            if (AffiliationsModifiers.ContainsKey(t))
            {
                modifiers = AffiliationsModifiers[t];
                return true;
            }
            return false;
        }
    } 
}