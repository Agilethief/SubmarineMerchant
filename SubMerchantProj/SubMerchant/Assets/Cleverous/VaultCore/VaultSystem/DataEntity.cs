// (c) Copyright Cleverous 2022. All rights reserved.

using UnityEngine;
using UnityEngine.Serialization;

namespace Cleverous.VaultSystem
{
    public abstract class DataEntity : ScriptableObject
    {
        [SerializeField]
        private int m_dbKey = int.MinValue;

        [FormerlySerializedAs("Title")]
        [SerializeField] 
        private string m_title = "Blank";
        [VaultFilterable]
        public string Title { get => m_title; set => m_title = value; }

        [TextArea]
        [FormerlySerializedAs("Description")]
        [SerializeField] 
        private string m_description;
        public string Description { get => m_description; set => m_description = value; }

        [SerializeField]
        public Sprite GetDataIcon => GetDataIconInternal();

        protected virtual void Reset()
        {
            Title = $"UNASSIGNED.{System.DateTime.Now.TimeOfDay.TotalMilliseconds}";
            Description = "";
        }

        /// <summary>
        /// Get the Database Key for this Entity.
        /// </summary>
        public int GetDbKey() { return m_dbKey; }

        /// <summary>
        /// Set the Database Key for this Entity.
        /// </summary>
        public void SetDbKey(int id) { m_dbKey = id; }

        /// <summary>
        /// Typically used in the Editor to display an icon for the Asset List. Can be used for other things at runtime if desired.
        /// </summary>
        protected virtual Sprite GetDataIconInternal()
        {
            return null;
        }
    }
}