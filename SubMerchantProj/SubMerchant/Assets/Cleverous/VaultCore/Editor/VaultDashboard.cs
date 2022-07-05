// (c) Copyright Cleverous 2022. All rights reserved.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using Cleverous.VaultSystem;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace Cleverous.VaultDashboard
{
    public class VaultDashboard : EditorWindow
    {
        // static from prefs
        private const string UxmlAssetName = "vault_dashboard_uxml";
        public static DataEntity CurrentSelectedAsset
        {
            get
            {
                if (Instance.m_currentSelectedAsset != null)
                {
                    return Instance.m_currentSelectedAsset;
                }

                string currentGuid = VaultEditorSettings.GetString(VaultEditorSettings.VaultData.CurrentAssetGuid);
                string currentPath = AssetDatabase.GUIDToAssetPath(currentGuid);
                DataEntity asset = AssetDatabase.LoadAssetAtPath<DataEntity>(currentPath);
                Instance.m_currentSelectedAsset = asset;
                return Instance.m_currentSelectedAsset;
            }
            private set
            {
                string currentPath = AssetDatabase.GetAssetPath(value);
                GUID currentGuid = AssetDatabase.GUIDFromAssetPath(currentPath);
                VaultEditorSettings.SetString(VaultEditorSettings.VaultData.CurrentAssetGuid, currentGuid.ToString());
                Instance.m_currentSelectedAsset = value;
            }
        }
        private DataEntity m_currentSelectedAsset;
        public static IDataGroup CurrentSelectedGroup
        {
            get
            {
                if (Instance.m_currentGroupSelected != null)
                {
                    return Instance.m_currentGroupSelected;
                }

                string currentName = VaultEditorSettings.GetString(VaultEditorSettings.VaultData.CurrentGroupName);
                VaultDataGroupFoldableButton button = Instance.GroupColumn.Q<VaultDataGroupFoldableButton>(currentName);
                if (button == null)
                {
                    // Debug.Log($"Failed to find a group button '{currentName}'.");
                    return null;
                }

                IDataGroup asset = button.DataGroup;
                if (asset == null)
                {
                    Debug.Log($"Failed to find group asset '{currentName}'.");
                }
                Instance.m_currentGroupSelected = asset;
                return Instance.m_currentGroupSelected;
            }
            private set
            {
                string title = value == null
                    ? "NULL GROUP"
                    : value.Title;
                VaultEditorSettings.SetString(VaultEditorSettings.VaultData.CurrentGroupName, title);
                Instance.m_currentGroupSelected = value;
            }
        }
        private IDataGroup m_currentGroupSelected;

        // static dynamic
        public static VaultDashboard Instance
        {
            get => m_instance == null ? null : m_instance;
            private set => m_instance = value;
        }
        private static VaultDashboard m_instance;

        // static constant
        private static readonly StyleColor ButtonInactive = new StyleColor(Color.gray);
        private static readonly StyleColor ButtonActive = new StyleColor(Color.white);

        // toolbar
        [SerializeField] protected Historizer Historizer;
        [SerializeField] public ToolbarSearchField SearchFieldForGroup; // TODO move these. 
        [SerializeField] public string AssetSearchCache;
        [SerializeField] public string TypeSearchCache;
        [SerializeField] protected string m_filterProperty;
        [SerializeField] protected string m_filterOperator;
        [SerializeField] protected string m_filterValue;
        public static bool SearchTypeIsDirty => Instance != null && Instance.SearchFieldForGroup != null && Instance.SearchFieldForGroup.value != Instance.TypeSearchCache;

        // columns
        [SerializeField] public VaultDataGroupColumn GroupColumn;
        [SerializeField] public VaultColumnOfAssets AssetColumn;
        [SerializeField] public VaultAssetInspector InspectorColumn;

        // wrappers for views
        [SerializeField] protected VisualElement WrapperForGroupContent;
        [SerializeField] protected VisualElement WrapperForAssetList;
        [SerializeField] protected VisualElement WrapperForAssetContent;
        [SerializeField] protected VisualElement WrapperForInspector;

        [SerializeField] protected ToolbarButton AssetNewButton;
        [SerializeField] protected ToolbarButton AssetDeleteButton;
        [SerializeField] protected ToolbarButton AssetCloneButton;
        [SerializeField] protected ToolbarButton AssetRemoveFromGroupButton;

        [SerializeField] protected DropdownField AssetFilterPropertyDropdown;
        [SerializeField] protected DropdownField AssetFilterOperation;
        [SerializeField] protected PropertyField AssetFilterPropertyValueField;
        [SerializeField] protected Toolbar AssetFilterBar;
        [SerializeField] public string AssetFilterValueString;
        [SerializeField] public float AssetFilterValueFloat;
        [SerializeField] public int AssetFilterValueInt;
        [SerializeField] protected VaultListFilter.FilterType AssetFilterType;

        [SerializeField] protected ToolbarButton GroupNewButton;
        [SerializeField] protected ToolbarButton RefreshButton;
        [SerializeField] protected ToolbarButton HelpButton;
        [SerializeField] protected ToolbarButton GroupDelButton;
        [SerializeField] protected Button IdSetButton;
        [SerializeField] protected IntegerField IdSetField;

        [MenuItem("Tools/Cleverous/Vault Dashboard %#d", priority = 0)]
        public static void Open()
        { 
            if (Instance != null)
            {
                FocusWindowIfItsOpen(typeof(VaultDashboard));
                return;
            }
             
            Instance = GetWindow<VaultDashboard>();
            Instance.titleContent.text = "Vault Dashboard";
            Instance.minSize = new Vector2(850, 200);
            Instance.Show();

            Instance.SetIdStartingPoint(VaultEditorSettings.GetInt(VaultEditorSettings.VaultData.StartingKeyId));
        }
        private void OnEnable()
        {
            Instance = this;
            DatabaseBuilder.CallbackAfterScriptReload();
            RebuildFull();
        }
        public void Update()
        {
            if (SearchTypeIsDirty)
            {
                AssetSearchCache = SearchFieldForGroup.value;
                VaultEditorSettings.SetString(VaultEditorSettings.VaultData.SearchAssets, AssetSearchCache);
                AssetColumn.ListAssetsBySearch();
            }

            if (GetAssetFilterPropertyName() != m_filterProperty)
            {
                m_filterProperty = GetAssetFilterPropertyName();
                UpdateAssetFilterPropertyField();
                AssetColumn.ListAssetsBySearch();
            }

            if (GetAssetFilterOperation().ToString() != m_filterOperator)
            {
                m_filterOperator = GetAssetFilterOperation().ToString();
                UpdateAssetFilterPropertyField();
                AssetColumn.ListAssetsBySearch();
            }

            if (GetAssetFilterPropertyValue() != m_filterValue)
            {
                m_filterValue = GetAssetFilterPropertyValue();
                AssetColumn.ListAssetsBySearch();
            }
        }

        private void LoadUxmlTemplate()
        {
            rootVisualElement.Clear();


            // load uxml and elements
            VisualTreeAsset visualTree = Resources.Load<VisualTreeAsset>(UxmlAssetName);
            visualTree.CloneTree(rootVisualElement);


            // find important parts and reference them
            WrapperForGroupContent = rootVisualElement.Q<VisualElement>("GC_CONTENT");
            WrapperForAssetContent = rootVisualElement.Q<VisualElement>("AC_CONTENT");
            WrapperForAssetList = rootVisualElement.Q<VisualElement>("ASSET_COLUMN");
            WrapperForInspector = rootVisualElement.Q<VisualElement>("INSPECT_COLUMN");
            SearchFieldForGroup = rootVisualElement.Q<ToolbarSearchField>("GROUP_SEARCH");

            Historizer = new Historizer(); 
            rootVisualElement.Q<VisualElement>("TB_HISTORY").Add(Historizer);


            // init group column buttons
            GroupNewButton = rootVisualElement.Q<ToolbarButton>("GC_NEW");
            GroupNewButton.style.backgroundImage = new StyleBackground(VaultEditorUtility.GetEditorImage("cyl_add"));
            GroupNewButton.clicked += CreateNewDataGroupCallback;

            GroupDelButton = rootVisualElement.Q<ToolbarButton>("GC_DEL");
            GroupDelButton.style.backgroundImage = new StyleBackground(VaultEditorUtility.GetEditorImage("cyl_del"));
            GroupDelButton.clicked += DeleteSelectedDataGroup;

            RefreshButton = rootVisualElement.Q<ToolbarButton>("GC_RELOAD");
            RefreshButton.style.backgroundImage = new StyleBackground(VaultEditorUtility.GetEditorImage("refresh"));
            RefreshButton.clicked += CallbackButtonRefresh;

            HelpButton = rootVisualElement.Q<ToolbarButton>("GC_HELP");
            HelpButton.style.backgroundImage = new StyleBackground(VaultEditorUtility.GetEditorImage("help"));
            HelpButton.clicked += CallbackButtonHelp;


            // init Asset Column Buttons
            AssetNewButton = WrapperForAssetList.Q<ToolbarButton>("AC_NEW");
            AssetNewButton.style.backgroundImage = new StyleBackground(VaultEditorUtility.GetEditorImage("cube_new"));
            AssetNewButton.clicked += CreateNewAssetCallback;

            AssetDeleteButton = WrapperForAssetList.Q<ToolbarButton>("AC_DELETE");
            AssetDeleteButton.style.backgroundImage = new StyleBackground(VaultEditorUtility.GetEditorImage("cube_del"));
            AssetDeleteButton.clicked += DeleteSelectedAsset;

            AssetCloneButton = WrapperForAssetList.Q<ToolbarButton>("AC_CLONE");
            AssetCloneButton.style.backgroundImage = new StyleBackground(VaultEditorUtility.GetEditorImage("clone"));
            AssetCloneButton.clicked += CloneSelectedAsset;

            AssetRemoveFromGroupButton = WrapperForAssetList.Q<ToolbarButton>("AC_GROUP_REMOVE");
            AssetRemoveFromGroupButton.style.backgroundImage = new StyleBackground(VaultEditorUtility.GetEditorImage("cyl_sub"));
            AssetRemoveFromGroupButton.clicked += RemoveAssetFromGroup;
            

            // init filtering interface
            AssetFilterBar = WrapperForAssetList.Q<Toolbar>("AC_FILTER_TOOLBAR");
            AssetFilterPropertyDropdown = WrapperForAssetList.Q<DropdownField>("AC_FILTER_DROPDOWN");
            AssetFilterPropertyDropdown.Remove(AssetFilterPropertyDropdown.Q<Label>()); // we have to remove the label from the dropdowns

            AssetFilterOperation = AssetFilterBar.Q<DropdownField>("AC_FILTER_OPERATION");
            AssetFilterOperation.choices = VaultListFilter.FilterOpSymbols;
            AssetFilterOperation.Remove(AssetFilterOperation.Q<Label>());
            IEnumerable<VisualElement> children = AssetFilterOperation.Children();
            children.ElementAt(0).style.maxWidth = 30;
            children.ElementAt(0).style.minWidth = 30;
            children.ElementAt(0).style.width = 30;
            

            // init footer
            IdSetButton = rootVisualElement.Q<Button>("ID_SET_BUTTON");
            IdSetButton.clicked += SetIdCallback;

            IdSetField = rootVisualElement.Q<IntegerField>("ID_SET_FIELD");
            IdSetField.SetValueWithoutNotify(VaultEditorSettings.GetInt(VaultEditorSettings.VaultData.StartingKeyId));

            WrapperForGroupContent.Add(GroupColumn);
            WrapperForAssetContent.Add(AssetColumn);
            WrapperForInspector.Add(InspectorColumn);


            // init split pane draggers
            // BUG - basically we have to do this because there is no proper/defined initialization for the drag anchor position.

            SplitView mainSplit = rootVisualElement.Q<SplitView>("MAIN_SPLIT");
            mainSplit.fixedPaneInitialDimension = 549;

            SplitView columnSplit = rootVisualElement.Q<SplitView>("FILTERS_PICK_SPLIT");
            columnSplit.fixedPaneInitialDimension = 250;

            SetIdStartingPoint(VaultEditorSettings.GetInt(VaultEditorSettings.VaultData.StartingKeyId));
        }

        public void RebuildFull()
        {
            LoadUxmlTemplate();
            Rebuild(true); 
        }
        public void Rebuild(bool fullRebuild = false)
        {
            //Debug.Log($"... Rebuild()");
            // search data
            SearchFieldForGroup.SetValueWithoutNotify(VaultEditorSettings.GetString(VaultEditorSettings.VaultData.SearchGroups));
            TypeSearchCache = SearchFieldForGroup.value;

            // rebuild
            RebuildGroupColumn(fullRebuild);
            RebuildInspectorColumn(fullRebuild);
            RebuildAssetColumn(fullRebuild);
            SetCurrentGroup(CurrentSelectedGroup);
        }

        private void RebuildGroupColumn(bool fullRebuild = false)
        {
            if (fullRebuild || GroupColumn == null)
            {
                WrapperForGroupContent.Clear();
                GroupColumn = new VaultFilterColumnInheritance();
                WrapperForGroupContent.Add(GroupColumn);
            }
            GroupColumn.VaultPanelReload();
        }
        private void RebuildAssetColumn(bool fullRebuild = false)
        {
            if (fullRebuild || AssetColumn == null)
            {
                WrapperForAssetContent.Clear();
                AssetColumn = new VaultColumnOfAssets();
                WrapperForAssetContent.Add(AssetColumn);
            }
            AssetColumn.VaultPanelReload();
        }
        private void RebuildInspectorColumn(bool fullRebuild = false)
        {
            if (fullRebuild || InspectorColumn == null)
            {
                InspectorColumn?.RemoveFromHierarchy();
                InspectorColumn = new VaultAssetInspector();
                WrapperForInspector.Add(InspectorColumn);
            }
            InspectorColumn.VaultPanelReload();
        }

        // filter property
        public string GetAssetFilterPropertyName()
        {
            return AssetFilterPropertyDropdown.choices[AssetFilterPropertyDropdown.index];
        }
        public string GetAssetFilterPropertyValue()
        {
            return AssetFilterType switch
            {
                VaultListFilter.FilterType.String => AssetFilterValueString,
                VaultListFilter.FilterType.Float => AssetFilterValueFloat.ToString(CultureInfo.InvariantCulture),
                VaultListFilter.FilterType.Int => AssetFilterValueInt.ToString(),
                _ => throw new ArgumentOutOfRangeException(nameof(AssetFilterType), AssetFilterType, null)
            };
        }
        public VaultListFilter.FilterType GetAssetFilterPropertyType()
        {
            // String
            // Float
            // Int
            // Enum (string list? layer field?)

            string pName = GetAssetFilterPropertyName();
            FieldInfo field = CurrentSelectedGroup.SourceType.GetField(pName);

            if (field != null)
            {
                //Debug.Log($"<color=lime>SELECT: `{pName}` - {field.FieldType} </color>");
                if (field.FieldType.IsAssignableFrom(typeof(string))) return VaultListFilter.FilterType.String;
                if (field.FieldType.IsAssignableFrom(typeof(float))) return VaultListFilter.FilterType.Float;
                if (field.FieldType.IsAssignableFrom(typeof(int))) return VaultListFilter.FilterType.Int;
            }
            else
            {
                PropertyInfo property = CurrentSelectedGroup.SourceType.GetProperty(pName);
                //Debug.Log($"<color=lime>SELECT: `{pName}` - {property.PropertyType} </color>");
                if (property.PropertyType.IsAssignableFrom(typeof(string))) return VaultListFilter.FilterType.String;
                if (property.PropertyType.IsAssignableFrom(typeof(float))) return VaultListFilter.FilterType.Float;
                if (property.PropertyType.IsAssignableFrom(typeof(int))) return VaultListFilter.FilterType.Int;
            }

            //Debug.Log("<color=red>Unknown Filter Type.</color>");
            return VaultListFilter.FilterType.String;
        }

        // filter operation
        public VaultListFilter.FilterOp GetAssetFilterOperation()
        {
            return (VaultListFilter.FilterOp) Math.Clamp(AssetFilterOperation.index, 0, int.MaxValue);
        }
        public void SetAssetFilterOperation(VaultListFilter.FilterOp op)
        {
            AssetFilterOperation.index = (int) op;
        }

        // filter update
        public void ResetAssetFilter()
        {
            
        }
        public void UpdateAssetFilterChoices()
        {
            // Property Choice
            List<string> results = VaultListFilter.GetFilterablePropertyNames();
            StringBuilder sb = new StringBuilder();
            foreach (var x in results)
            {
                sb.Append(x + ", ");
            }
            //Debug.Log($"<color=orange>Updating. Valids: {sb}</color>");

            if (AssetFilterPropertyDropdown.choices != null && AssetFilterPropertyDropdown.choices.Count != results.Count) 
                AssetFilterPropertyDropdown.index = 0;

            AssetFilterPropertyDropdown.choices = results;

            int index = results.FindIndex(0, x => x == AssetFilterPropertyDropdown.text);
            if (index > 0)
            {
                AssetFilterPropertyDropdown.index = index;
                AssetFilterPropertyDropdown.value = results[index];
            }
            else
            {
                AssetFilterPropertyDropdown.index = 0;
                AssetFilterPropertyDropdown.value = results[0];
            }
        }
        public void UpdateAssetFilterPropertyField()
        {
            PropertyField pf = AssetFilterBar.Q<PropertyField>();
            if (pf != null) AssetFilterBar.Remove(pf);

            AssetFilterType = GetAssetFilterPropertyType();
            SerializedProperty property;
            switch (AssetFilterType)
            {
                case VaultListFilter.FilterType.String:
                    //Debug.Log("Created String Field");
                    property = Editor.CreateEditor(this).serializedObject.FindProperty("AssetFilterValueString");
                    break;
                case VaultListFilter.FilterType.Float:
                    //Debug.Log("Created Float Field");
                    property = Editor.CreateEditor(this).serializedObject.FindProperty("AssetFilterValueFloat");
                    break;
                case VaultListFilter.FilterType.Int:
                    //Debug.Log("Created Int Field");
                    property = Editor.CreateEditor(this).serializedObject.FindProperty("AssetFilterValueInt");
                    break;
                default: throw new ArgumentOutOfRangeException();
            }

            AssetFilterPropertyValueField = new PropertyField(property, "")
            {
                viewDataKey = "filter_property_field",
                name = "FILTER_PROPERTY_FIELD"
            };
            AssetFilterPropertyValueField.style.minWidth = 100;
            AssetFilterPropertyValueField.style.maxWidth = 200;
            AssetFilterPropertyValueField.bindingPath = property.propertyPath;
            AssetFilterPropertyValueField.Bind(property.serializedObject);

            AssetFilterBar.Add(AssetFilterPropertyValueField);
        }

        public void SetCurrentGroup(IDataGroup group)
        {
            if (group == null) return;
            bool isCustom = group.GetType() == typeof(VaultCustomDataGroup);
            AssetRemoveFromGroupButton.SetEnabled(isCustom);
            AssetRemoveFromGroupButton.style.unityBackgroundImageTintColor = isCustom ? ButtonActive : ButtonInactive;

            CurrentSelectedGroup = group;
            UpdateAssetFilterChoices();
            UpdateAssetFilterPropertyField();
            GroupColumn.SelectButtonByTitle(group.Title);
            AssetColumn.ListAssetsByGroup(true);
            // TODO RESET FILTER FIELD?
        }
        public void SetCurrentInspectorAsset(DataEntity asset)
        {
            CurrentSelectedAsset = asset;
            InspectorColumn.VaultPanelReload();
            Historizer.AddAndHistorize();
        }
        public void InspectAssetRemote(Object asset, Type t)
        {
            if (asset == null && t == null) return;
            if (t == null) return;

            if (Instance == null) Open();
            Instance.Focus();
            // TODO RESET FILTER FIELD?

            VisualElement button = WrapperForGroupContent.Q<VisualElement>(t.Name);
            IVaultDataGroupButton buttonInterface = (IVaultDataGroupButton) button;
            if (buttonInterface != null)
            {
                buttonInterface.SetAsCurrent();
                GroupColumn.ScrollTo(button);
            }
            
            if (asset != null) AssetColumn.Pick((DataEntity)asset);
            InspectorColumn.VaultPanelReload();
        }

        /// <summary>
        /// The Dashboard button calls this to create a new asset in the current group.
        /// </summary>
        private void CreateNewAssetCallback()
        {
            if (CurrentSelectedGroup.SourceType.IsAbstract)
            {
                bool confirm = EditorUtility.DisplayDialog(
                    "Group Error",
                    "Selected Class is abstract! We can't create a new asset in abstract class groups. Choose a valid class and create a new Data Asset, then you can store it in a Custom Group.",
                    "Ok");
                if (confirm) return;
            }
            CreateNewAsset();
        }
        /// <summary>
        /// Create a new asset with the current group Type.
        /// </summary>
        /// <returns></returns>
        public void CreateNewAsset()
        {
            AssetColumn.NewAsset(CurrentSelectedGroup.SourceType);
        }
        /// <summary>
        /// Create a new asset with a specific Type.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public DataEntity CreateNewAsset(Type t)
        {
            Debug.Log($"Create new asset with specific Type: {t.Name}");
            DataEntity newAsset = AssetColumn.NewAsset(t);
            DatabaseBuilder.Reload();
            Instance.RebuildFull();
            return newAsset;
        }
        public void CloneSelectedAsset()
        {
            AssetColumn.CloneSelection();
        }
        public void DeleteSelectedAsset()
        {
            AssetColumn.DeleteSelection();
        }

        public void SetIdStartingPoint(int id)
        {
            Vault.Db.SetIdStartingValue(id);
            VaultEditorSettings.SetInt(VaultEditorSettings.VaultData.StartingKeyId, id);
            if (IdSetField != null) IdSetField.value = id;
            if (Vault.Db != null) EditorUtility.SetDirty(Vault.Db);
        }
        private void SetIdCallback()
        {
            SetIdStartingPoint(IdSetField.value);
        }

        public void RemoveAssetFromGroup()
        {
            CurrentSelectedGroup.RemoveEntity(CurrentSelectedAsset.GetDbKey());
            AssetColumn.VaultPanelReload();
        }
        public void CreateNewDataGroupCallback()
        {
            CreateNewDataGroup();
        }
        public void DeleteSelectedDataGroup()
        {
            if (CurrentSelectedGroup == null) return;
            if (CurrentSelectedGroup.GetType() != typeof(VaultCustomDataGroup)) return;
            VaultCustomDataGroup customGroup = (VaultCustomDataGroup) CurrentSelectedGroup;
            if (customGroup == null) return;

            bool confirm = EditorUtility.DisplayDialog(
                "Delete Custom Group",
                $"Are you sure you want to permanently delete '{CurrentSelectedGroup.Title}'?",
                "Delete",
                "Abort");
            if (!confirm) return;

            InspectAssetRemote(null, typeof(object));
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(customGroup));
            CurrentSelectedGroup = null;
            Instance.Rebuild();
        }
        public VaultCustomDataGroup CreateNewDataGroup()
        {
            VaultCustomDataGroup result = (VaultCustomDataGroup)AssetColumn.NewAsset(typeof(VaultCustomDataGroup));
            GroupColumn.VaultPanelReload();
            InspectAssetRemote(result, typeof(VaultCustomDataGroup));
            return null;
        }
        public void CallbackButtonRefresh()
        {
            RebuildFull();
        }
        public void CallbackButtonHelp()
        {
            Application.OpenURL("https://lanefox.gitbook.io/vault/");
        }
    }
}