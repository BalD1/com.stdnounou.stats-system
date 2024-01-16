#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace StdNounou.Stats.Samples
{
    public class InspectorStatModifier_MonoHandler : MonoBehaviour
    {
        [SerializeField] private SO_EStatModifierData modifierData;
        [SerializeField] private EMonoStatsHandler statsHandler;

        public void ApplyModifier()
        {
            statsHandler.StatsHandler.TryAddModifier(modifierData, out var result);
        }
    } 
}

#if UNITY_EDITOR
namespace StdNounou.Stats.Samples
{
    [CustomEditor(typeof(InspectorStatModifier_MonoHandler))]
    public class ED_InspectorStatModifier : Editor
    {
        private InspectorStatModifier_MonoHandler targetScript;

        private void OnEnable()
        {
            targetScript = (InspectorStatModifier_MonoHandler)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Apply"))
                targetScript.ApplyModifier();
        }
    }  
}
#endif
