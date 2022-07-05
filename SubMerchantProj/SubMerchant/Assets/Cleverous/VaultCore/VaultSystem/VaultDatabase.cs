// (c) Copyright Cleverous 2022. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Cleverous.VaultSystem
{
    [CreateAssetMenu(fileName = "New Database", menuName = "Cleverous/Vault/Database", order = 0)]
    public class VaultDatabase : ScriptableObject, ISerializationCallbackReceiver
    {
        public const string VaultDatabaseName = "VaultCoreDatabase";
        public Dictionary<int, DataEntity> Data = new Dictionary<int, DataEntity>(); // no boxing required

        // serialized forcefully between dict/list.
        [SerializeField] private List<int> m_dataKeys = new List<int>();
        [SerializeField] private List<DataEntity> m_dataVals = new List<DataEntity>();

        // naturally serializes.
        [SerializeField] private List<VaultStaticDataGroup> m_staticGroups = new List<VaultStaticDataGroup>();

        private int m_uniqueIdIterator;

        /// <summary>
        /// Callback which will get the move data from the Dictionary (which cannot serialize) to the key/value Lists (which can be serialized)
        /// </summary>
        public virtual void OnBeforeSerialize()
        {
            // // // Move data from Dictionary to List. // // //

            m_dataKeys.Clear();
            m_dataVals.Clear();
            foreach (KeyValuePair<int, DataEntity> d in Data)
            {
                m_dataKeys.Add(d.Key);
                m_dataVals.Add(d.Value);
            }

            //Debug.Log($"<color=orange>DB BEFORE SERIALIZE : {GetEntityCount()} Entities in dictionary</color>");
        }

        /// <summary>
        /// Callback which will move the data from the key/value Lists (which have been serialized) and create the Dictionaries (which can't serialize).
        /// </summary>
        public void OnAfterDeserialize()
        {
            // // // Move data from List to Dictionary. // // //

            // Standard Data
            Data = new Dictionary<int, DataEntity>();
            for (int i = 0; i < m_dataKeys.Count; i++)
            {
                Data.Add(m_dataKeys[i], m_dataVals[i]);
            }

            //Debug.Log($"<color=lime>DB AFTER DESERIALIZE : {GetEntityCount()} Entities in dictionary</color>");
        }

        /// <summary>
        /// Get a DataEntity from the Database. Uses a dictionary lookup which is 0(1).
        /// </summary>
        /// <param name="key">The unique key for the DataEntity.</param>
        /// <returns>The DataEntity reference, or Null if the key doesn't exist in the Database.</returns>
        public DataEntity Get(int key)
        {
            return Data.ContainsKey(key) ? Data[key] : null;
        }

        /// <summary>
        /// Do not use linear string lookups as they are painfully inefficient O(n). Instead, get the Key from DataEntity.GetDbKey() and pass that for O(1) lookup.
        /// </summary>
        /// <param name="title">The long, specific, error prone and possibly duplicate string name of the Item.</param>
        /// <returns>A waste of time.</returns>
        [Obsolete("Do not use linear string lookups on the entire database as they are very inefficient O(n). Instead, get the Key from DataEntity.GetDbKey() and pass that for O(1) lookup.")]
        public DataEntity Get(string title)
        {
            return Data.Single(x => x.Value.Title == title).Value;
        }

        /// <summary>
        /// Get all DataEntities that inherit from a specific type. This is a linear search that will cast every entry in the DB to type T. Very slow, cache your results and avoid frequent use.
        /// </summary>
        /// <typeparam name="T">The Type you want collected. Must derive from DataEntity.</typeparam>
        /// <returns>A list of all DataEntities in the database that can successfully cast to T</returns>
        public List<T> GetAll<T>()
        {
            return m_dataVals.OfType<T>().Select(x => (T) Convert.ChangeType(x, typeof(T))).ToList();
        }

        /// <summary>
        /// Add an item to the Database. Using this at runtime is not really useful since it doesn't persist between sessions, but maybe so if you are hotloading in custom content at runtime every session.
        /// </summary>
        /// <param name="data">The data reference.</param>
        /// <param name="generateNewId">If true, a new ID will be generated for the item.</param>
        public virtual void Add(DataEntity data, bool generateNewId = true)
        {
            if (data == null) return;

            int id = generateNewId ? GenerateUniqueId() : data.GetDbKey();
            if (Data.ContainsKey(id)) return;
            data.SetDbKey(id);
            Data.Add(id, data);
            
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(data);
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }

        /// <summary>
        /// Remove an item from the Database. Using this at runtime is not really useful since it doesn't persist between sessions, but maybe so if you are hotloading in custom content at runtime every session.
        /// </summary>
        /// <param name="data">The data reference.</param>
        public virtual void Remove(DataEntity data)
        {
            if (Data.ContainsKey(data.GetDbKey()))
            {
                Data.Remove(data.GetDbKey());
            }
        }

        /// <summary>
        /// Remove an item from the Database. This is a fast O(1) operation. Using this at runtime is not really useful since it doesn't persist between sessions, but maybe so if you are hotloading in custom content at runtime every session.
        /// </summary>
        /// <param name="id">The unique key ID for the item.</param>
        public virtual void Remove(int id)
        {
            if (Data.ContainsKey(id))
            {
                Data.Remove(id);
            }
        }

        /// <summary>
        /// Get the group responsible for the type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual VaultStaticDataGroup GetStaticGroup<T>() where T : DataEntity
        {
            return m_staticGroups.Single(x => x.SourceType == typeof(T));
        }

        /// <summary>
        /// Get the group responsible for the type T.
        /// </summary>
        /// <returns></returns>
        public virtual VaultStaticDataGroup GetStaticGroup(Type t)
        {
            return m_staticGroups.Single(x => x.SourceType == t);
        }

        public virtual List<VaultStaticDataGroup> GetAllStaticGroups()
        {
            List<VaultStaticDataGroup> result = m_staticGroups.Where(x => x != null).ToList();
            return result;
        }

        /// <summary>
        /// Set the group responsible for the given type.
        /// </summary>
        /// <param name="identifier">The class itself.</param>
        public virtual void SetStaticGroup(VaultStaticDataGroup identifier)
        {
            foreach (VaultStaticDataGroup x in m_staticGroups.Where(x => x.SourceType == identifier.SourceType))
            {
                m_staticGroups.Remove(identifier);
            }
            m_staticGroups.Add(identifier);
        }

        /// <summary>
        /// Clear the entire database of all Data content.
        /// </summary>
        public virtual void ClearData()
        {
            Data.Clear();
            m_dataKeys.Clear();
            m_dataVals.Clear();
        }

        /// <summary>
        /// Clear the entire database of all Group content.
        /// </summary>
        public virtual void ClearStaticGroups()
        {
            m_staticGroups.Clear();
        }

        /// <summary>
        /// Get the number of Entities in the Database. Does not include groups.
        /// </summary>
        /// <returns></returns>
        public virtual int GetEntityCount()
        {
            return Data.Count;
        }
        
        /// <summary>
        /// Generate the next unique ID.
        /// </summary>
        /// <returns>A unique ID from the Database which can be assigned to a new DataEntity</returns>
        public int GenerateUniqueId()
        {
            int result = 0;
            bool done = false;
            while (!done)
            {
                result = m_uniqueIdIterator++;
                if (!Data.ContainsKey(result)) done = true;
            }
            return result;
        }

        /// <summary> 
        /// Set it iterator value to the arg value. This is safe, and new items will still be given unique IDs since it checks the DB for usage before assignment.
        /// </summary>
        /// <param name="id">The starting value for new id's.</param>
        public void SetIdStartingValue(int id)
        {
            m_uniqueIdIterator = id;
        }
    }
}