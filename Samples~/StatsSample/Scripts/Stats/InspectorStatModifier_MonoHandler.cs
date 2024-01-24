#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace StdNounou.Stats.Samples
{
    public class InspectorStatModifier_MonoHandler : MonoBehaviour
    {
        [SerializeField] private SO_StatModifierData modifierData;
        [SerializeField] private MonoStatsHandler statsHandler;

        public void ApplyModifier()
        {
            statsHandler.StatsHandler.TryAddModifier(modifierData, out _, out _);
        }
    } 
}

#if UNITY_EDITOR
namespace StdNounou.Stats.Samples
{
    [CustomEditor(typeof(InspectorStatModifier_MonoHandler))]
    public class ED_InspectorStatModifier : UnityEditor.Editor
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
