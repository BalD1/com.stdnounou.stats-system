using System.Text;
using UnityEditor;
using StdNounou.Core.Editor;

namespace StdNounou.Stats.Editor
{
    public static class StdNounouStatsEditorUtils
    {
        [MenuItem("StdNounou/CreateFile/StatsEnum")]
        private static void CreateStatsEnum()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("namespace StdNounou.Stats \n");
            sb.Append("{\n");
            sb.Append("    public enum E_StatsKeys\n");
            sb.Append("    {\n");
            sb.Append("        Health,\n");
            sb.Append("        Damages,\n");
            sb.Append("        DamageReduction,\n");
            sb.Append("        CritChances,\n");
            sb.Append("        CritMultiplier,\n");
            sb.Append("        AttackCooldown,\n");
            sb.Append("        Speed,\n");
            sb.Append("        Weight\n");
            sb.Append("        KnockbackForce\n");
            sb.Append("    }\n");
            sb.Append("}");

            EditorCreates.TryCreateFolder("StdNounou", "Assets");
            EditorCreates.TryCreateFolder("Keys", "Assets/StdNounou");
            EditorCreates.TryCreateFolder("Stats", "Assets/StdNounou/Keys");
            EditorCreates.CreateFile("StatsKeys", "Assets/StdNounou/Keys/Stats/", ".cs", sb.ToString(), false);

            //create assembly def with stdnounou.stats.runtime
            EditorCreates.CreateAssemblyDefinition("StdNounou.stats.runtime_ref", "Assets/StdNounou/Keys/Stats/", true, "GUID:6ddfdff7ac37b354883336b81ebb5114");
        }
    }
}