// (c) Copyright Cleverous 2022. All rights reserved.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cleverous.VaultSystem
{
    [Serializable]
    public class VaultStaticDataGroup : IDataGroup
    { 
        public string Title
        {
            get => SourceType.Name;
            set => Debug.Log($"Tried to set a static group Title to {value}. But, we can't set the Title on static groups.");
        }
        public Type SourceType
        {
            get => Type.GetType(m_typeName);
            set => m_typeName = value.AssemblyQualifiedName;
        }
        [SerializeField] [HideInInspector]
        private string m_typeName = typeof(DataEntity).AssemblyQualifiedName;
        
        public List<DataEntity> Content
        {
            get => m_data;
            set => m_data = value;
        }
        [SerializeField]
        private List<DataEntity> m_data = new List<DataEntity>();

        public VaultStaticDataGroup(Type t)
        {
            SourceType = t;
        }

        public void AddEntity(DataEntity data)
        {
            Content.Add(data);
        }
        public void RemoveEntity(int key)
        {
            for (int i = 0; i < Content.Count; i++)
            {
                if (Content[i].GetDbKey() == key)
                {
                    Content.RemoveAt(i);
                }
            }
        }
        public void Sanitize()
        {
            Content.RemoveAll(x => x == null);
        }
    }
}