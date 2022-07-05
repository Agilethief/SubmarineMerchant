// (c) Copyright Cleverous 2022. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cleverous.VaultSystem;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Cleverous.VaultDashboard
{
    public class DatabaseBuilder : IPreprocessBuildWithReport
    {
        /// <summary>
        /// <para>Vault will not read types from any assemblies starting with these prefixes.</para>
        /// <para>This is done to improve compile times by ignoring namespaces that will never
        /// contain a Type which inherits from DataEntity.</para>
        /// <para>Please check your assembly names if you aren't seeing content in the Type List.</para>
        /// </summary>
        public static readonly string[] AssemblyBlacklist =
        {
            "System",
            "Mono.",
            "Unity.",
            "UnityEngine",
            "UnityEditor",
            "mscorlib",
            "SyntaxTree",
            "netstandard",
            "nunit",
            "AssetStoreTools",
            "ExCSS" 
        };

        public int callbackOrder => 100;
        public void OnPreprocessBuild(BuildReport report)
        {
            Reload();
        }

        public static void CallbackAfterScriptReload()
        {
            if (Vault.Db == null)
            {
                Debug.LogWarning("Is this the first time loading Vault? You may want to restart the editor and run the Data Upgrader at 'Tools/Cleverous/Vault/Data Upgrader'.");
                return;
            }
            Reload();
        }

        /// <summary>
        /// Rebuild lists of static groups, custom groups, and all data entities.
        /// </summary>
        [MenuItem("Tools/Cleverous/Vault/DB Soft Reload", priority = 10)]
        public static void Reload()
        {
            FindDataEntities();
            FindStaticGroups();

            EditorUtility.SetDirty(Vault.Db);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // check if any data has a key of int.MinValue, which is default unassigned and suggests a problem or old data.
            if (Vault.Db.Get(int.MinValue))
            {
                bool pressedOk = EditorUtility.DisplayDialog(
                    "Invalid entries detected",
                    "A entry in the Database with the default state int.MinValue (-2147483648) was found. This may be because Vault was just upgraded, and is normal. \n\n" +
                    "Please set the DB Key Starting Value in the Vault Dashboard footer and then run '/Tools/Cleverous/Vault/Data Upgrader' to upgrade any DB keys.",
                    "Ok",
                    "Open Online Help");
                if (!pressedOk)
                {
                    Application.OpenURL("https://lanefox.gitbook.io/vault/vault-inventory/create-new-database-items/manage-unique-asset-ids#set-your-local-unique-id");
                }
            }

            // if (VaultDashboard.Instance != null) VaultDashboard.Instance.RebuildFull();
            //Debug.Log($"<color=orange> DB Reload Callback Completed : Entities[{Vault.Db.GetEntityCount()}], Static Groups[{Vault.Db.GetAllStaticGroups().Count}], Custom Groups[{Vault.Db.GetAll<VaultCustomDataGroup>().Count}]</color>");
        }

        private static void FindStaticGroups()
        {
            Vault.Db.ClearStaticGroups();

            Assembly vaultAssy = Assembly.GetAssembly(typeof(DataEntity));
            AssemblyName vaultAssyName = vaultAssy.GetName();
            List<string> processedAssyNames = new List<string>();

            // ** ASSEMBLY TOP LEVEL
            foreach (Assembly assy in AppDomain.CurrentDomain.GetAssemblies())
            {
                string assyName = assy.GetName().Name;

                // ignore dynamic, blacklisted and duplicates
                if (assy.IsDynamic
                    || AssemblyBlacklist.Any(ignored => assyName.StartsWith(ignored))
                    || processedAssyNames.Any(n => n == assyName)) continue;

                // ** TOP ASSEMBLY REFERERENCED LEVEL
                foreach (AssemblyName referencedAssembly in assy.GetReferencedAssemblies())
                {
                    // check the assembly to see if it references Cleverous.VaultSystem
                    if (referencedAssembly.Name != vaultAssyName.Name) continue;

                    // if it does reference it, we can get all the classes inside it and make groups.
                    List<VaultStaticDataGroup> validDataClasses = BuildStaticGroupsFromAssembly(assy);

                    // find all of the assets for that group Type and add them into the DB
                    foreach (VaultStaticDataGroup x in validDataClasses) Vault.Db.SetStaticGroup(x);
                    processedAssyNames.Add(referencedAssembly.Name);
                }
            }

            // include VaultSystem
            List<VaultStaticDataGroup> vaultClasses = BuildStaticGroupsFromAssembly(vaultAssy);
            foreach (VaultStaticDataGroup x in vaultClasses) Vault.Db.SetStaticGroup(x);
            processedAssyNames.Add(vaultAssy.GetName().Name);
        }
        private static List<VaultStaticDataGroup> BuildStaticGroupsFromAssembly(Assembly assy)
        {
            List<VaultStaticDataGroup> result = new List<VaultStaticDataGroup>();

            // ** ASSEMBLY LEVEL
            IEnumerable<IGrouping<string, Type>> groups = assy.GetExportedTypes()
                .Where(t => t.IsSubclassOf(typeof(DataEntity)) || t == typeof(DataEntity))
                .GroupBy(t => t.Namespace);

            // Find all of the valid types and make group instances for them.
            // ** NAMESPACE LEVEL
            foreach (IGrouping<string, Type> namespaceGroup in groups)
            {
                // ** TYPE LEVEL
                foreach (Type type in namespaceGroup)
                {
                    VaultStaticDataGroup group = new VaultStaticDataGroup(type) {SourceType = type};
                    group.Content = GetAllDataEntitiesOfTypeInProject(type);
                    result.Add(group);
                }
            }

            // send back the list of types in this assembly
            return result;
        }
        protected static List<VaultCustomDataGroup> ProcessAssembly(Assembly assy)
        {
            List<VaultCustomDataGroup> groupListResult = new List<VaultCustomDataGroup>();

            // ** ASSEMBLY LEVEL
            IEnumerable<IGrouping<string, Type>> groups = assy.GetExportedTypes()
                .Where(t => t.IsSubclassOf(typeof(DataEntity)) || t == typeof(DataEntity))
                .GroupBy(t => t.Namespace);

            // ** NAMESPACE LEVEL
            foreach (IGrouping<string, Type> namespaceGroup in groups)
            {
                // ** TYPE LEVEL
                foreach (Type type in namespaceGroup)
                {
                    VaultCustomDataGroup group = ScriptableObject.CreateInstance<VaultCustomDataGroup>();
                    group.Title = type.Name;
                    group.Content = GetAllDataEntitiesOfTypeInProject(type);
                    group.SourceType = type;
                    groupListResult.Add(group);
                    Debug.Log($"<color=green>Discovered {group.SourceType} as a static group type</color>"); 
                }
            }
            return groupListResult;
        }

        private static void FindDataEntities()
        {
            List<DataEntity> list = new List<DataEntity>();
            string[] guids = AssetDatabase.FindAssets($"t:{typeof(DataEntity)}");
            list.AddRange(guids.Select(guid => AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(DataEntity)) as DataEntity));

            Vault.Db.ClearData();
            foreach (DataEntity x in list)
            {
                Vault.Db.Add(x, false);
            }

            EditorUtility.SetDirty(Vault.Db);
        }

        [MenuItem("Tools/Cleverous/Vault/Data Key Upgrader (safe)", priority = 100)]
        public static void DataUpgrader()
        {
            bool proceed = EditorUtility.DisplayDialog(
                "Upgrade Data",
                "This will find every DataEntity in the project that has a key of int.MinValue and assign it a new one.",
                "Proceed",
                "Abort");
            if (!proceed) return;

            if (Vault.Db == null)
            {
                Debug.Log("DB Not found. Please restart the editor and open Vault Dashboard.");
                return;
            }

            List<DataEntity> data = GetAllDataEntitiesOfTypeInProject(typeof(DataEntity));
            int changed = 0;
            foreach (DataEntity x in data.Where(x => x.GetDbKey() == int.MinValue))
            {
                x.SetDbKey(Vault.Db.GenerateUniqueId());
                EditorUtility.SetDirty(x);
                changed++;
            }

            Reload();

            EditorUtility.DisplayDialog(
                "Complete",
                $"Changed {changed} entity keys.",
                "Ok");
        }

        [MenuItem("Tools/Cleverous/Vault/Data Key Reset (danger)", priority = 100)]
        public static void DataReset()
        {
            bool proceed = EditorUtility.DisplayDialog(
                "Reset Data Keys",
                "This will find every DataEntity in the project and assign a new DB Key to each one.",
                "Proceed",
                "Abort");
            if (!proceed) return;

            if (Vault.Db == null)
            {
                Debug.Log("DB Not found. Please restart the editor and open Vault Dashboard.");
                return;
            }

            List<DataEntity> data = GetAllDataEntitiesOfTypeInProject(typeof(DataEntity));
            int changed = 0;
            foreach (DataEntity x in data)
            {
                x.SetDbKey(Vault.Db.GenerateUniqueId());
                EditorUtility.SetDirty(x);
                changed++;
            }

            Reload();

            EditorUtility.DisplayDialog(
                "Complete",
                $"Changed {changed} entity keys.",
                "Ok");
        }

        /// <summary>
        /// Forces a refresh of assets serialization.
        /// </summary>
        [MenuItem("Tools/Cleverous/Vault/Reimport Assets - By Type (safe)", priority = 100)]
        public static void ReimportAllByType()
        {
            bool confirm = EditorUtility.DisplayDialog(
                "Reimport Vault Asset Files", 
                $"Reimport all of the Vault Data Assets?\n\n" +
                $"This reimports all DataEntity type Assets. Won't fix issues related to mismatching class/file names.\n\n This is generally a safe operation.",
                "Proceed", 
                "Abort!");

            if (!confirm) return;

            int count = 0;
            AssetDatabase.StartAssetEditing();
            try
            {
                string storage = VaultEditorUtility.GetPathToVaultCoreStorageFolder();
                if (storage[storage.Length - 1] == '/') storage = storage.Remove(storage.Length - 1);
                string[] files = AssetDatabase.FindAssets("t:DataEntity", new[] { storage });
                for (int i = 0; i < files.Length; i++)
                {
                    EditorUtility.DisplayProgressBar("Importing...", AssetDatabase.GUIDToAssetPath(files[i]), (float)i / files.Length);
                    AssetDatabase.ImportAsset(AssetDatabase.GUIDToAssetPath(files[i]), ImportAssetOptions.ForceUpdate);
                    Debug.Log($"{AssetDatabase.GUIDToAssetPath(files[i])}");
                    count++;
                }
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
                EditorUtility.ClearProgressBar();
                EditorUtility.DisplayDialog(
                    "Done reimporting",
                    $"{count} assets were reimported and logged to the console.",
                    "Great");
            }
        }
        
        /// <summary>
        /// Forces a refresh of assets serialization.
        /// </summary>
        [MenuItem("Tools/Cleverous/Vault/Reimport Assets - By Name (safe)", priority = 100)]
        public static void ReimportAllByName()
        {
            bool confirm = EditorUtility.DisplayDialog(
                "Reimport Vault Asset Files", 
                "Reimport all of the Vault Data Assets?\n\n" +
                "This reimports all files with names including 'Data-' which is the built-in prefix for saved Vault Files.\n\n This is generally a safe operation.", 
                "Proceed", 
                "Abort");

            if (!confirm) return;

            int count = 0;
            AssetDatabase.StartAssetEditing();
            try
            {
                string storage = VaultEditorUtility.GetPathToVaultCoreStorageFolder();
                if (storage[storage.Length - 1] == '/') storage = storage.Remove(storage.Length - 1);
                string[] files = AssetDatabase.FindAssets("Data-", new[] { storage });
                for (int i = 0; i < files.Length; i++)
                {
                    EditorUtility.DisplayProgressBar("Importing...", AssetDatabase.GUIDToAssetPath(files[i]), (float)i / files.Length);
                    AssetDatabase.ImportAsset(AssetDatabase.GUIDToAssetPath(files[i]), ImportAssetOptions.ForceUpdate);
                    Debug.Log($"{AssetDatabase.GUIDToAssetPath(files[i])}");
                    count++;
                }
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
                EditorUtility.ClearProgressBar();
                EditorUtility.DisplayDialog(
                    "Done reimporting",
                    $"{count} assets were reimported and logged to the console.",
                    "Great");
            }
        }
        
        [MenuItem("Tools/Cleverous/Vault/Cleanup Vault (semi-safe)", priority = 100)]
        public static void CleanupStorageFolder()
        {
            bool confirm = EditorUtility.DisplayDialog(
                "Cleanup Vault",
                "This will check all asset files with the 'Data-' prefix and ensure validity. Invalid files can be deleted. This is primarily for identifying or removing assets which have broken script connections due to class name mismatches or class deletions. \n\n" +
                "You will be able to confirm delete/skip for each file individually.\n\n"  +
                "You may NOT want to do this if the data found is broken accidentally and you're trying to restore it. This does not restore data, it validates assets and offers deletion if they are problematic. While this cleans up the project, it does DELETE the data asset file.\n", 
                "Proceed", 
                "Abort");

            if (!confirm) return;

            int found = 0;
            int deleted = 0;
            int failed = 0;
            int ignored = 0;
            AssetDatabase.StartAssetEditing();
            try
            {
                string storage = VaultEditorUtility.GetPathToVaultCoreStorageFolder();
                if (storage[storage.Length - 1] == '/') storage = storage.Remove(storage.Length - 1);
                string[] files = AssetDatabase.FindAssets("Data-", new[] { storage });
                for (int i = 0; i < files.Length; i++)
                {
                    EditorUtility.DisplayProgressBar("Scanning...", AssetDatabase.GUIDToAssetPath(files[i]), (float)i / files.Length);

                    string path = AssetDatabase.GUIDToAssetPath(files[i]);
                    DataEntity dataFile = AssetDatabase.LoadAssetAtPath<DataEntity>(AssetDatabase.GUIDToAssetPath(files[i]));
                    if (dataFile == null)
                    {
                        found++;

                        // how the heck do i get the object if we're literally dealing with objects that don't cast.
                        //EditorGUIUtility.PingObject();

                        bool deleteFaulty = EditorUtility.DisplayDialog(
                            "Faulty file found",
                            $"{path}\n\n" +
                            "This file seems to be broken. Please check:\n\n" +
                            "* File is actually a Vault Data file.\n" +
                            "* Class file still exists.\n" +
                            "* Class filename matches class name.\n" +
                            "* Assemblies are not black-listed.\n\n" +
                            "What do you want to do?", "Delete file", "Ignore file");

                        if (deleteFaulty)
                        {
                            bool success = AssetDatabase.DeleteAsset(path);
                            if (success) deleted++;
                            else failed++;
                        }
                        else ignored++;
                    }
                }
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
                EditorUtility.ClearProgressBar();
                EditorUtility.DisplayDialog(
                    "Done cleaning.",
                    $"{found} assets were faulty.\n" +
                    $"{deleted} assets were deleted.\n" +
                    $"{failed} assets failed to delete.\n" +
                    $"{ignored} assets were ignored.\n",
                    "Excellent");
            }

        }

        public static List<VaultCustomDataGroup> GetAllCustomDataGroupAssets()
        {
            List<VaultCustomDataGroup> result = new List<VaultCustomDataGroup>();

            foreach (KeyValuePair<int, DataEntity> group in Vault.Db.Data)
            {
                if (!(group.Value is VaultCustomDataGroup value)) continue;

                //Debug.Log($"Added '{group}' to custom group list");
                result.Add(value);
            }

            return result;
        }

        public static List<T> GetAllAssetsInProject<T>() where T : DataEntity
        {
            // The AssetDatabase does not work correctly during callback for assembly reload and script reload.
            // Use a direct method instead during those times.

            List<T> list = new List<T>();
            string[] guids = AssetDatabase.FindAssets($"t:{typeof(T)}");
            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                Debug.Log($"Group asset found: {assetPath}"); 
                DataEntity asset = (DataEntity)AssetDatabase.LoadAssetAtPath(assetPath, typeof(T));
                Debug.Log($"Added '{asset}' to custom group list");
                list.Add(asset as T);
            }
            return list;
        }
        public static List<DataEntity> GetAllDataEntitiesOfTypeInProject(Type t)
        {
            List<DataEntity> list = new List<DataEntity>();
            string[] guids = AssetDatabase.FindAssets($"t:{t.Name}");
            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                DataEntity asset = (DataEntity)AssetDatabase.LoadAssetAtPath(assetPath, typeof(DataEntity));
                list.Add(asset);
            }
            return list;
        }
    }
}