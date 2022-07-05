// (c) Copyright Cleverous 2022. All rights reserved.

using UnityEditor;
using UnityEngine;

namespace Cleverous.VaultDashboard
{
    public static class VaultEditorSettings
    {
        public enum VaultData
        {
            CurrentAssetGuid,       // guid of the current selected asset
            CurrentGroupName,       // guid of the current selected group 
            BreadcrumbBarGuids,     // guids
            SelectedAssetGuids,     // guids
            SearchGroups,           // the content of the search group field
            SearchAssets,           // the content of the search asset field
            StartingKeyId,          // the content of the search asset field
        }

        public static int GetInt(VaultData data)
        {
            int result = EditorPrefs.GetInt(data.ToString());
            return result;
        }
        public static string GetString(VaultData data)
        {
            string result = EditorPrefs.GetString(data.ToString());
            return result;
        }

        public static void SetInt(VaultData data, int value)
        {
            EditorPrefs.SetInt(data.ToString(), value);
        }
        public static void SetString(VaultData data, string value)
        {
            EditorPrefs.SetString(data.ToString(), value);
        }
    }
}