// (c) Copyright Cleverous 2022. All rights reserved.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cleverous.VaultSystem
{
    public class VaultCustomDataGroup : DataEntity, IDataGroup
    { 
        public Type SourceType
        {
            get => Type.GetType(m_typeName);
            set => m_typeName = value.AssemblyQualifiedName;
        }
        [SerializeField] 
        [HideInInspector] 
        private string m_typeName = typeof(DataEntity).AssemblyQualifiedName;

        public List<DataEntity> Content
        {
            get => m_data;
            set => m_data = value;
        }
        [SerializeField]
        private List<DataEntity> m_data = new List<DataEntity>();

        protected override void Reset()
        {
            base.Reset();
            Title = "New Custom Data Group";
            Description = "Used to store a list of custom Data Entity types for easy reference.";
        }

        public virtual void AddEntity(DataEntity entity)
        {
            if (Content.Contains(entity)) return;
            Content.Add(entity);
            EditorHandleDirty();
        }
        public virtual void RemoveEntity(int key)
        {
            for (int i = 0; i < Content.Count; i++)
            {
                if (Content[i].GetDbKey() == key)
                {
                    Content.RemoveAt(i);
                }
            }
            EditorHandleDirty();
        }
        public virtual void Sanitize()
        {
            Content.RemoveAll(x => x == null);
        }
        protected virtual void EditorHandleDirty()
        {
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
#endif
        }
    }
}