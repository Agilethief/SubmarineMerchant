// (c) Copyright Cleverous 2022. All rights reserved.

#if UNITY_EDITOR
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Cleverous.VaultDashboard
{
    public static class DefineSymbolUtil
    {
        public const string Symbol = "CLV_VAULT";

        [MenuItem("Tools/Cleverous/Vault/Install Vault Define Symbol", priority = 900)]
        public static void InstallDefines()
        {
            RemoveDefine(Symbol);
            AddDefine(Symbol);
        }

        [UnityEditor.Callbacks.DidReloadScripts]
        public static void Verify()
        {
            if (!PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup)
                .Contains(Symbol))
            {
                Debug.Log($"Define Symbol '{Symbol}' was not detected so it has been installed. Unity will recompile.");
                InstallDefines();
            }
        }

        public static void AddDefine(string def)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup));
            if (sb.Length == 0) sb.Append(def + ";");
            else sb.Append($"{(sb[sb.Length - 1] == ';' ? "" : ";")}{def}");
            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, sb.ToString());
        }
        public static void RemoveDefine(string def)
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup,
                PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup).Replace(def, ""));
        }
    }
}
#endif