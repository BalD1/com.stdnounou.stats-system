using UnityEditor;
using StdNounou.Core.Editor;
using StdNounou.Tick;

namespace StdNounou.Stats
{
    [CustomEditor(typeof(SO_StatModifierData))]
    public class ED_SO_StatModifierData : UnityEditor.Editor
    {
        private SO_StatModifierData targetScript;

        private bool showDefaultInspector = false;

        private void OnEnable()
        {
            targetScript = (SO_StatModifierData)target;
        }

        public override void OnInspectorGUI()
        {
            ReadOnlyDraws.EditorScriptDraw(typeof(ED_SO_StatModifierData), this);
            base.DrawDefaultInspector();

            if (!targetScript.Temporary) return;

            EditorGUILayout.LabelField($"Time in seconds : {targetScript.TicksLifetime * TickManager.TICK_TIMER_MAX}s", EditorStyles.boldLabel);
        }
    }
}