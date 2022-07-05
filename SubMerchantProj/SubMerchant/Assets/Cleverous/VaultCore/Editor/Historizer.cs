// (c) Copyright Cleverous 2022. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cleverous.VaultSystem;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
 
namespace Cleverous.VaultDashboard
{
    public class Historizer : VisualElement
    {
        private List<DataEntity> m_history;
        private readonly VisualElement m_breadcrumbBar;
        private List<Button> m_buttons; // todo highlight if current selection is inside history bar.

        private const int MaxHistoryItems = 7;
        private const int MaxNameLength = 16;

        public Historizer()
        {
            m_breadcrumbBar = new ToolbarBreadcrumbs();
            m_buttons = new List<Button>();
            Add(m_breadcrumbBar);

            style.flexGrow = 1;
            style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);

            // VaultDashboard.OnCurrentAssetChanged += AddAndHistorize;
            
            RestoreHistory();
            BuildBreadcrumbs();
        }

        /// <summary>
        /// Store all of the current history object GUIDs into Settings.
        /// </summary>
        public void AddAndHistorize()
        {
            if (m_history == null) m_history = new List<DataEntity>();
            if (m_history.Count > 0 && m_history.Last() == VaultDashboard.CurrentSelectedAsset) return;
            if (m_history.Contains(VaultDashboard.CurrentSelectedAsset)) return;

            m_history.Add(VaultDashboard.CurrentSelectedAsset);
            if (m_history.Count > MaxHistoryItems)
            { 
                m_history.Remove(m_history.First());
            }

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < m_history.Count; i++)
            {
                DataEntity assetFile = m_history[i];
                sb.Append(AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(assetFile)) + "|");
            }
            VaultEditorSettings.SetString(VaultEditorSettings.VaultData.BreadcrumbBarGuids, sb.ToString());
            BuildBreadcrumbs();
        }

        /// <summary>
        /// Read Settings to obtain all history GUIDs and create the History List of objects from them.
        /// </summary>
        private void RestoreHistory()
        {
            m_history = new List<DataEntity>();
            string guidblob = VaultEditorSettings.GetString(VaultEditorSettings.VaultData.BreadcrumbBarGuids);
            if (string.IsNullOrEmpty(guidblob)) return;

            string[] split = guidblob.Split('|');
            foreach (string guid in split)
            {
                if (guid == string.Empty || guid.Contains('|')) continue;
                DataEntity target = AssetDatabase.LoadAssetAtPath<DataEntity>(AssetDatabase.GUIDToAssetPath(guid));
                if (target != null) m_history.Add(target);
            }
        }

        private void BuildBreadcrumbs()
        {
            m_breadcrumbBar.Clear();
            m_buttons = new List<Button>();
            StyleBackground crumbFirst = (Texture2D)EditorGUIUtility.IconContent("breadcrump left").image;
            StyleBackground crumb = (Texture2D)EditorGUIUtility.IconContent("breadcrump mid").image;

            if (m_history == null || m_history.Count == 0) return;
            for (int i = 0; i < m_history.Count; i++)
            {
                if (m_history[i] == null) continue;
                int index = i;

                string title = "      blank";
                if (m_history[i].Title.Length > 0)
                {
                    title = m_history[i].Title.Length > MaxNameLength
                        ? "      " + m_history[i].Title.Substring(0, MaxNameLength - 2) + "..."
                        : "      " + m_history[i].Title;
                }

                Button btn = new Button(() => GoToHistoryIndex(index));
                btn.style.paddingBottom = 0;
                btn.style.paddingLeft = 0;
                btn.style.paddingRight = 0;
                btn.style.paddingTop = 0;
                btn.style.borderBottomLeftRadius = 0;
                btn.style.borderBottomRightRadius = 0;
                btn.style.borderTopRightRadius = 0;
                btn.style.borderTopLeftRadius = 0;
                btn.style.borderBottomWidth = 0;
                btn.style.borderLeftWidth = 0;
                btn.style.borderRightWidth = 0;
                btn.style.borderTopWidth = 0;
                btn.style.unitySliceLeft = 12;
                btn.style.unitySliceRight = 15;
                btn.style.marginTop = 0;
                btn.style.marginBottom = 0;
                btn.style.marginLeft = -15; 
                btn.style.marginRight = 0;
                btn.style.flexGrow = 1;
                btn.style.flexShrink = 1;
                btn.style.width = 500;
                btn.style.unityTextAlign = new StyleEnum<TextAnchor>(TextAnchor.MiddleLeft);
                btn.style.unityBackgroundImageTintColor = new StyleColor(Color.white);
                btn.style.unityBackgroundScaleMode = new StyleEnum<ScaleMode>(ScaleMode.StretchToFill);
                btn.style.backgroundColor = new StyleColor(Color.clear);
                btn.style.backgroundImage = i > 0 ? crumb : crumbFirst;

                btn.text = title;

                m_buttons.Add(btn);
                m_breadcrumbBar.Add(btn);
            }
        }
         
        public void GoToHistoryIndex(int index)
        {
            if (m_history[index] == null) return;
            VaultDashboard.Instance.InspectAssetRemote(m_history[index], m_history[index].GetType());
        }
    }
}