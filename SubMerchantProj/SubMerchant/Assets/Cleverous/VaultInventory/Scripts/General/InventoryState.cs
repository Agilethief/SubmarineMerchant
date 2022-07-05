// (c) Copyright Cleverous 2020. All rights reserved.

using System;
using System.Collections.Generic;
using Cleverous.VaultSystem;
using UnityEngine;
using UnityEngine.Serialization;

namespace Cleverous.VaultInventory
{
    [Serializable]
    public partial class InventoryState
    {  
        /// <summary>
        /// The Vault Index ID of the Inventory Configuration.
        /// </summary>
        [FormerlySerializedAs("ConfigIndex")]
        public int ConfigDbKey;
        /// <summary>
        /// A list of Vault Index IDs to identify items in each slot.
        /// </summary>
        [FormerlySerializedAs("ItemIndexes")]
        public List<int> ItemDbKeys;
        /// <summary>
        /// A list of int's to indicate the stack size in each slot.
        /// </summary>
        public List<int> ItemStackCounts;

        public InventoryState(Inventory source, List<RootItemStack> content)
        {
            ConfigDbKey = source.Configuration.GetDbKey();
            ItemDbKeys = new List<int>();
            ItemStackCounts = new List<int>();

            foreach (RootItemStack t in content)
            {
                ItemDbKeys.Add(t != null ? t.Source.GetDbKey() : -1);
                ItemStackCounts.Add(t != null ? t.StackSize : 0);
            }
        }

        public virtual string ToJson()
        {
            return JsonUtility.ToJson(this, true);
        }
        public static InventoryState FromJson(string json)
        {
            return JsonUtility.FromJson<InventoryState>(json);
        }
    }
}