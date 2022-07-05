// (c) Copyright Cleverous 2022. All rights reserved.

using UnityEditor;
using UnityEngine;

namespace Cleverous.VaultDashboard
{
    public static class VaultEditorUtility
    {
        private const string VaultLocatorFileGuid = "cb441f7bdab6b61459aa19cfd0138c36";
        private const string VaultItemPath = "VaultCoreStorage/";
        private static string m_vaultPath;

        public static string GetPathToVaultCoreRootFolder()
        {
            if (string.IsNullOrEmpty(m_vaultPath)) return GetPathToVaultCoreRootFolderCached();

            string assetsFolderPath = Application.dataPath;
            assetsFolderPath = assetsFolderPath.Substring(0, assetsFolderPath.Length - 6);
            bool exists = System.IO.File.Exists(assetsFolderPath + m_vaultPath);

            return exists 
                ? m_vaultPath 
                : GetPathToVaultCoreRootFolderCached();
        }
        private static string GetPathToVaultCoreRootFolderCached()
        {
            m_vaultPath = AssetDatabase.GUIDToAssetPath(VaultLocatorFileGuid);
            m_vaultPath = m_vaultPath.Replace("VaultLocatorFile.cs", "");
            return m_vaultPath;
        }
        public static string GetPathToVaultCoreStorageFolder()
        {
            string result = GetPathToVaultCoreRootFolder() + VaultItemPath;
            return result;
        }

        public static Texture2D GetEditorImage(string title)
        {
            return AssetDatabase.LoadAssetAtPath<Texture2D>($"{GetPathToVaultCoreRootFolder()}/Editor/Icons/{title}.png");
        }
    }
}