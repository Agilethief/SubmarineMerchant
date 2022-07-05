// (c) Copyright Cleverous 2022. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEditor.UIElements; 
using UnityEngine.UIElements;

namespace Cleverous.VaultDashboard
{
    public class VaultAssetInspector : VaultDashboardColumn
    {
        protected SerializedObject TargetSerializedObject;
        private readonly ScrollView m_contentWindow;

        public VaultAssetInspector()
        {
            TargetSerializedObject = GetSerializedObj();
            m_contentWindow = new ScrollView();
            m_contentWindow.style.flexShrink = 1f;
            m_contentWindow.style.flexGrow = 1f;
            m_contentWindow.style.paddingBottom = 10;
            m_contentWindow.style.paddingLeft = 10;
            m_contentWindow.style.paddingRight = 10;
            m_contentWindow.style.paddingTop = 10;

            this.name = "Asset Inspector";
            this.viewDataKey = "ASSET_INSPECTOR";
            this.style.flexShrink = 1f;
            this.style.flexGrow = 1f;
            this.style.paddingBottom = 10;
            this.style.paddingLeft = 10;
            this.style.paddingRight = 10;
            this.style.paddingTop = 10;
            this.Add(m_contentWindow);
        }

        public override void VaultPanelReload()
        {
            m_contentWindow.Clear();
            if (VaultDashboard.CurrentSelectedAsset == null)
            {
                InspectNothing();
                return;
            }

            TargetSerializedObject = GetSerializedObj();

            bool success = BuildInspectorProperties(TargetSerializedObject, m_contentWindow);
            if (success) m_contentWindow.Bind(TargetSerializedObject); // TODO BUG
        }

        public void InspectNothing()
        {
            m_contentWindow.Clear();
            m_contentWindow.Add(new Label { text = " ⓘ Asset Inspector" });
            m_contentWindow.Add(new Label("\n\n    ⚠ Please select an asset from the column to the left."));
        }
        private static SerializedObject GetSerializedObj()
        {
            return VaultDashboard.CurrentSelectedAsset == null 
                ? null 
                : Editor.CreateEditor(VaultDashboard.CurrentSelectedAsset).serializedObject;
        }
        private static bool BuildInspectorProperties(SerializedObject obj, VisualElement wrapper)
        {
            if (obj == null || wrapper == null) return false;
            wrapper.Add(new Label { text = " ⓘ Asset Inspector" });

            // if Unity ever makes their InspectorElement work then we can just use that instead of
            // butchering through the object and making each field manually. (since 2019)

            /*
            InspectorElement inspector = new InspectorElement(obj);
            inspector.style.flexGrow = 1;
            inspector.style.flexShrink = 1;
            inspector.style.alignSelf = new StyleEnum<Align>(Align.Stretch);
            inspector.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
            inspector.style.alignItems = new StyleEnum<Align>(Align.Stretch);
            inspector.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);
            wrapper.Add(inspector);
            return true;
            */

            SerializedProperty iterator = obj.GetIterator();
            Type targetType = obj.targetObject.GetType();
            List<MemberInfo> members = new List<MemberInfo>(targetType.GetMembers());

            if (!iterator.NextVisible(true)) return false;
            do
            {
                bool isHidden = false;
                PropertyField propertyField = new PropertyField(iterator.Copy())
                {
                    name = "PropertyField:" + iterator.propertyPath
                };

                MemberInfo member = members.Find(x => x.Name == propertyField.bindingPath);
                if (member != null)
                {
                    Attribute[] hides = member.GetCustomAttributes(typeof(HideInInspector)).ToArray();
                    foreach (Attribute _ in hides)
                    {
                        isHidden = true;
                    }

                    if (!isHidden)
                    {
                        CustomAttributeData[] allAttributes = member.CustomAttributes.ToArray();
                        Attribute[] headers = member.GetCustomAttributes(typeof(HeaderAttribute)).ToArray();
                        Attribute[] spaces = member.GetCustomAttributes(typeof(SpaceAttribute)).ToArray();

                        // BUG seems like if there is more than 1 attribute, they all get drawn fine by Unity but 1 alone requires manual drawing?
                        if (allAttributes.Length < 2)
                        {
                            foreach (Attribute h in headers)
                            {
                                HeaderAttribute actual = (HeaderAttribute) h;
                                Label header = new Label {text = actual.header};
                                header.style.unityFontStyleAndWeight = FontStyle.Bold;
                                wrapper.Add(new Label {text = " "});
                                wrapper.Add(header);
                            }

                            foreach (Attribute _ in spaces)
                            {
                                wrapper.Add(new Label {text = " "});
                            }
                        }
                    }
                }

                // for the db key field
                if (iterator.propertyPath == "m_dbKey" && obj.targetObject != null)
                {
                    // build the container
                    VisualElement container = new VisualElement();
                    container.style.flexGrow = 1;
                    container.style.flexShrink = 1;
                    container.style.alignItems = new StyleEnum<Align>(Align.Stretch);
                    container.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);

                    propertyField.SetEnabled(false);
                    propertyField.style.flexGrow = 1;
                    propertyField.style.flexShrink = 1;

                    // draw it
                    container.Add(propertyField);
                    wrapper.Add(container);
                }
                // if this property is the script field
                if (iterator.propertyPath == "m_Script" && obj.targetObject != null)
                {
                    // build the container
                    VisualElement container = new VisualElement();
                    container.style.flexGrow = 1;
                    container.style.flexShrink = 1;
                    container.style.alignItems = new StyleEnum<Align>(Align.Stretch);
                    container.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);

                    propertyField.SetEnabled(false);
                    propertyField.style.flexGrow = 1;
                    propertyField.style.flexShrink = 1;

                    // build the focus script button
                    Button focusButton = new Button(() => EditorGUIUtility.PingObject(obj.FindProperty("m_Script").objectReferenceValue));
                    focusButton.text = "☲";
                    focusButton.style.minWidth = 20;
                    focusButton.style.maxWidth = 20;
                    focusButton.tooltip = "Ping this Script";                    
                    
                    // build the focus object button
                    Button focusAsset = new Button(() => EditorGUIUtility.PingObject(obj.targetObject));
                    focusAsset.text = "☑";
                    focusAsset.style.minWidth = 20;
                    focusAsset.style.maxWidth = 20;
                    focusAsset.tooltip = "Ping this Asset";

                    // draw it
                    container.Add(propertyField);
                    container.Add(focusButton);
                    container.Add(focusAsset);
                    wrapper.Add(container);
                }
                // if it isn't the script field, just add the property field like normal.
                else if (!isHidden) wrapper.Add(propertyField);
            }
            while (iterator.NextVisible(false));
            return true;
        }
    }
}