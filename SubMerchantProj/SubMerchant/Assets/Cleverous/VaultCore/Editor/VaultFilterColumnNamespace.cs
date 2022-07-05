// (c) Copyright Cleverous 2022. All rights reserved.
/*

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cleverous.VaultSystem;
using UnityEngine.UIElements;

namespace Cleverous.VaultDashboard
{
    public class VaultDataGroupColumnNamespace : VaultDataGroupColumn
    {
        protected static ScrollView ScrollElement;
        protected static List<VaultTypeAssemblyFoldoutAssembly> TypeColumnElements;

        public override void Rebuild()
        {
            Clear();
            ScrollElement = new ScrollView();
            ScrollElement.style.flexGrow = 1;
            this.Add(ScrollElement);

            VaultAssy = Assembly.GetAssembly(typeof(DataEntity));
            VaultAssyName = VaultAssy.GetName();

            AllValidTypesCache = new List<Type>();
            AllButtonsCache = new List<IVaultDataGroupButton>();

            TypeColumnElements?.Clear();
            TypeColumnElements = new List<VaultTypeAssemblyFoldoutAssembly>();

            if (!IsSubscribed)
            {
                VaultDashboard.OnTypeFilterChangeComplete += FilterBySearchBar;
                VaultDashboard.OnTypeSearch += FilterBySearchBar;
            }

            IsSubscribed = true;

            // loop through the assemblies available
            foreach (Assembly assy in AppDomain.CurrentDomain.GetAssemblies())
            {
                string assyName = assy.GetName().Name;
                if (assy.IsDynamic) continue; // ignore dynamics
                if (Blacklist.Any(ignored => assyName.StartsWith(ignored))) continue; // ignore blacklisted
                if (TypeColumnElements.Any(n => n.AssemblyName == assyName)) continue; // ignore duplicates

                // ** ASSEMBLY LEVEL
                foreach (AssemblyName q in assy.GetReferencedAssemblies())
                {
                    if (q.Name != VaultAssyName.Name) continue; // ignore anything not explicitly referencing Vault.
                    ProcessAssembly(assy, q, assyName);
                }
            }

            // Include the Vault assembly, which doesn't reference itself and wouldn't normally be included.
            ProcessAssembly(VaultAssy, VaultAssyName, VaultAssyName.Name);

            // alphabetically sort the list
            TypeColumnElements.Sort((x, y) => string.CompareOrdinal(x.AssemblyName, y.AssemblyName));

            // add everything in the list to the wrapper so it can be seen.
            foreach (VaultTypeAssemblyFoldoutAssembly e in TypeColumnElements)
            {
                ScrollElement.Add(e);
            }

            // persistence
            // CurrentTypeName = VaultEditorSettings.GetString(VaultEditorSettings.VaultData.CurrentTypeName);
            foreach (VaultTypeAssemblyFoldoutAssembly x in TypeColumnElements)
            {
                VaultDataGroupButtonType result = x.Q<VaultDataGroupButtonType>(CurrentTypeName);
                if (result == null) continue;

                result.SetAsCurrent();
                break;
            }
        }

        protected virtual void ProcessAssembly(Assembly assy, AssemblyName q, string assyName)
        {
            // ** ASSEMBLY LEVEL
            bool includeAssyInView = false;

            string title = assy.GetName().Name;
            VaultTypeAssemblyFoldoutAssembly assyFoldout = new VaultTypeAssemblyFoldoutAssembly(title);
            assyFoldout.viewDataKey = title;

            IEnumerable<IGrouping<string, Type>> groups = assy.GetExportedTypes()
                .Where(t => t.IsSubclassOf(typeof(DataEntity)) || t == typeof(DataEntity))
                .GroupBy(t => t.Namespace);

            // ** NAMESPACE LEVEL
            foreach (IGrouping<string, Type> namespaceGroup in groups)
            {
                // this switch is to handle when the Assembly is the same name
                // as the Namespace. Using AsmDefs this is 100% of the time.
                // This will just skip creating a redundant folder for the NS.
                // Minor code duplication...
                if (namespaceGroup.Key == assyName)
                {
                    // ** TYPE LEVEL
                    foreach (Type type in namespaceGroup)
                    {
                        includeAssyInView = true;
                        VaultDataGroupButtonType b = assyFoldout.AddTypeButton(type, assyFoldout);
                        AllValidTypesCache.Add(type);
                        AllButtonsCache.Add(b);
                    }
                }
                else
                {
                    VaultTypeAssemblyFoldoutAssembly namespaceFoldout = new VaultTypeAssemblyFoldoutAssembly($"{namespaceGroup.Key ?? "Global Namespace"} ({namespaceGroup.Count()})");
                    namespaceFoldout.viewDataKey = namespaceGroup.Key;
                    assyFoldout.Add(namespaceFoldout);

                    // ** TYPE LEVEL
                    foreach (Type type in namespaceGroup)
                    {
                        includeAssyInView = true;
                        VaultDataGroupButtonType b = namespaceFoldout.AddTypeButton(type, namespaceFoldout);
                        AllValidTypesCache.Add(type);
                        AllButtonsCache.Add(b);
                    }
                }
            }

            if (includeAssyInView) TypeColumnElements.Add(assyFoldout);
        }

        public override void ScrollTo(VisualElement button)
        {
            ScrollElement.ScrollTo(button);
        }

        public override void Filter(string f)
        {
            if (string.IsNullOrEmpty(f))
            {
                foreach (IVaultDataGroupButton x in AllButtonsCache)
                {
                    x.SetIsHighlighted(true);
                }
            }
            else
            {
                foreach (IVaultDataGroupButton x in AllButtonsCache)
                {
                    // x.SetIsHighlighted(x.SourceType.Name.ToLower().Contains(f.ToLower()));
                }
            }
        }
        public override void FilterBySearchBar()
        {
            Filter(VaultDashboard.SearchFieldForType.value);
        }
    }
}
*/