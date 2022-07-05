// (c) Copyright Cleverous 2022. All rights reserved.

using Cleverous.VaultSystem;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(SpritePreviewAttribute))]
public class SpritePropertyDrawer : PropertyDrawer
{
    private const float TextSize = 70;

    public override float GetPropertyHeight(SerializedProperty p, GUIContent label)
    {
        return p.objectReferenceValue != null 
            ? TextSize 
            : base.GetPropertyHeight(p, label);
    }

    public override void OnGUI(Rect pos, SerializedProperty p, GUIContent label)
    {
        EditorGUI.BeginProperty(pos, label, p);

        if (p.objectReferenceValue != null)
        {
            pos.width = EditorGUIUtility.labelWidth;
            GUI.Label(pos, p.displayName);
            pos.x += pos.width;
            pos.width = TextSize;
            pos.height = TextSize;
            p.objectReferenceValue = EditorGUI.ObjectField(pos, p.objectReferenceValue, typeof(Sprite), false);
        }
        else
        {
            GUI.Label(pos, p.displayName);
            EditorGUI.PropertyField(pos, p, true);
        }
        EditorGUI.EndProperty();
    }
}