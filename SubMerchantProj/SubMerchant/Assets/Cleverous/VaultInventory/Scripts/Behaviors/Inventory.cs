// (c) Copyright Cleverous 2020. All rights reserved.

using System;
using System.Collections.Generic;
using Cleverous.VaultSystem;
using Cleverous.NetworkImposter;
using UnityEngine;
using UnityEngine.Serialization;

#if MIRROR && !FISHNET
using Mirror;
#elif FISHNET
using FishNet.Object;
using Command = FishNet.Object.ServerRpcAttribute;
using ClientRpc = FishNet.Object.ObserversRpcAttribute;
#endif


namespace Cleverous.VaultInventory
{
    /// <summary>
    /// This class manages the List side of the inventory and fires events when the list changes.
    /// </summary>
    public partial class Inventory : NetworkBehaviour
    {
        public bool IsInitialized { get; protected set; }

#if MIRROR && !FISHNET
        [Obsolete("Renamed to InventoryOwner. Update your code to use InventoryOwner instead.")]
        public IUseInventory Owner
        {
            get { return InventoryOwner; }
            set { InventoryOwner = value; }
        }
#endif
        public IUseInventory InventoryOwner;
        /// <summary>
        /// When a slot is changed in any way. Could be empty, increased stack size, or an entirely different item - could even be new slot space!
        /// </summary>
        public Action<int> OnChanged;
        /// <summary>
        /// When any item is added to the inventory.
        /// </summary>
        public Action<RootItemStack> OnItemAdded;
        /// <summary>
        /// When any item is removed from the inventory.
        /// </summary>
        public Action<RootItemStack> OnItemRemoved;
        /// <summary>
        /// When this Inventory component is destroyed by the engine.
        /// </summary>
        public Action<Inventory> OnDestroyed;
        /// <summary>
        /// When the Inventory has been fully initialized.
        /// </summary>
        public Action OnInitialized;

        [AssetDropdown(typeof(InventoryConfig))]
        [FormerlySerializedAs("Configuration")]
        [SerializeField]
        private InventoryConfig m_configuration;
        public virtual InventoryConfig Configuration
        {
            get => m_configuration;
            set => m_configuration = value;
        }

        protected List<RootItemStack> Content { get; set; }

        protected List<SlotRestriction> Restrictions;
        public int MaxSlots
        {
            get => m_maxSlots;
            set
            {
                if (value < m_maxSlots || value == m_maxSlots) return;

                int oldCount = m_maxSlots;
                m_maxSlots = value;
                for (int i = oldCount; i < m_maxSlots; i++)
                {
                    Content.Add(null);
                    OnChanged?.Invoke(i);
                }
            }
        }
        private int m_maxSlots;

        /// <summary>
        /// Initialize the inventory.
        /// </summary>
        /// <param name="owner">The interface that owns this Inventory. If no one initializes it, then it doesn't work.</param>
        /// <param name="resetContent">When false, the Content will not be modified. Used when loading from a specific <see cref="InventoryState"/> and you do not want the <see cref="Content"/> to be written to.</param>
        /// <param name="config">The slot count and slot restrictions for the inventory.</param>
        /// <param name="silent">When true, the function will not(!) invoke <see cref="OnInitialized"/> and will not(!) set <see cref="IsInitialized"/> to true. Useful for overrides and granularity.</param>
        public virtual void Initialize(IUseInventory owner, bool resetContent, InventoryConfig config = null, bool silent = false)
        {
            //Debug.Log($"<B>INVENTORY</B> - <color=yellow>Inventory Init (BASE)</color>");
            
            // set configuration
            if (config == null && Configuration != null) config = Configuration;
            else if (config != null) Configuration = config;
            else config = ScriptableObject.CreateInstance<InventoryConfig>();

            // set owner
            InventoryOwner = owner;

            // set slot restrictions
            Restrictions = new List<SlotRestriction>();
            m_maxSlots = config.SlotRestrictions.Length > 0 ? config.SlotRestrictions.Length : 1;
            for (int i = 0; i < config.SlotRestrictions.Length; i++)
            {
                Restrictions.Add(config.SlotRestrictions[i]);
            }

            // set content
            if (resetContent)
            {
                // list must always be at capacity and all empty nodes must be null.
                Content = new List<RootItemStack>();
                for (int i = 0; i < config.SlotRestrictions.Length; i++)
                {
                    Content.Add(null);
                }
            }

            if (!silent)
            {
                IsInitialized = true;
                OnInitialized?.Invoke();
            }
        }

        /// <summary>
        /// Initialize this inventory from an <see cref="InventoryState"/> which has references to the config and content stacks.
        /// </summary>
        public virtual void Initialize(IUseInventory owner, InventoryState state)
        {
            //Debug.Log("<B>INVENTORY</B> - <color=yellow>Inventory Init (BASE - FROM STATE)</color>");

            Configuration = (InventoryConfig)Vault.Get(state.ConfigDbKey);
            Content = new List<RootItemStack>();

            for (int i = 0; i < state.ItemDbKeys.Count; i++)
            {
                if (state.ItemDbKeys[i] == -1)
                {
                    Content.Add(null);
                    continue;
                }

                Content.Add(new RootItemStack(
                    (RootItem)Vault.Get(state.ItemDbKeys[i]),
                    state.ItemStackCounts[i]));
            }

            Initialize(owner, false, Configuration, silent:true);
            IsInitialized = true;
            OnInitialized?.Invoke();
        }

        /// <summary>
        /// Get the configuration and content of this Inventory converted into an <see cref="InventoryState"/> which can be JSON'd.
        /// </summary>
        public virtual InventoryState ToState()
        {
            return new InventoryState(this, Content);
        }



        public override void OnStopServer()
        {
#if FISHNET
            base.OnStopServer();
#endif
            m_maxSlots = 0;
            Content = new List<RootItemStack>();
            Restrictions = new List<SlotRestriction>();
            IsInitialized = false;
        }
        public override void OnStopClient()
        {
#if FISHNET
            base.OnStopClient();
#endif
            m_maxSlots = 0;
            Content = new List<RootItemStack>();
            Restrictions = new List<SlotRestriction>();
            IsInitialized = false;
        }



        // Public Queries
        /// <summary>
        /// Read the content at the given index.
        /// </summary>
        /// <param name="index">The index to get</param>
        /// <returns>The content of this Inventory at the given index</returns>
        public virtual RootItemStack Get(int index)
        {
            return Content == null || Content[index] == null || Content[index].Source == null
                ? new RootItemStack(null, 0) 
                : Content[index];
        }

        /// <summary>
        /// Searches the Inventory to see if it has an amount of a specific item by matching title.
        /// </summary>
        /// <param name="item">The item to look for</param>
        /// <param name="amount">The minimum amount required to return true</param>
        /// <returns>True if the Inventory contains at least the amount of items provided. False otherwise.</returns>
        public virtual bool Contains(RootItem item, int amount)
        {
            List<int> indices = new List<int>();
            int verifiedCount = 0;

            for (int i = 0; i < MaxSlots; i++)
            {
                if (Get(i).Source == null) continue;
                if (Get(i).Source.Title == item.Title)
                {
                    indices.Add(i);
                }
            }

            for (int i = 0; i < indices.Count; i++)
            {
                verifiedCount += Get(indices[i]).StackSize;
            }

            return verifiedCount >= amount;
        }

        /// <summary>
        /// Get all slots with the matching restriction
        /// </summary>
        /// <param name="restriction"></param>
        /// <returns>All indexes that match the provided SlotRestriction</returns>
        public virtual int[] GetAllSlotIndexOfType(SlotRestriction restriction)
        {
            List<int> results = new List<int>();
            for (int i = 0; i < Configuration.SlotRestrictions.Length; i++)
            {
                if (Configuration.SlotRestrictions[i] == restriction) results.Add(i);
            }
            return results.ToArray();
        }

        /// <summary>
        /// Get the first slot with the matching restriction
        /// </summary>
        /// <param name="restriction"></param>
        /// <param name="mustBeEmpty">If true, only empty slots are returned.</param>
        /// <returns>The first found index that matches the provided SlotRestriction. If no match is found then -1 is returned.</returns>
        public virtual int GetFirstSlotIndexOfType(SlotRestriction restriction, bool mustBeEmpty)
        {
            for (int i = 0; i < Configuration.SlotRestrictions.Length; i++)
            {
                if (Configuration.SlotRestrictions[i] == restriction)
                {
                    if (mustBeEmpty && Content[i] == null) return i;
                    if (!mustBeEmpty) return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Count how many of the given item are in this inventory.
        /// </summary>
        /// <param name="item">The item you want to count.</param>
        /// <returns>Available stack size of the item in the inventory.</returns>
        public virtual int GetCountOfItem(RootItem item)
        {
            return GetItemCount(item.GetDbKey());
        }

        /// <summary>
        /// Count how many of the given item are in this inventory.
        /// </summary>
        /// <param name="itemDbKey">The DB Key of the item you want to count.</param>
        /// <returns>Available stack size of the item in the inventory.</returns>
        public virtual int GetCountOfItem(int itemDbKey)
        {
            return DoCount(itemDbKey);
        }

        /// <summary>
        /// Count the number of slots with no content in them.
        /// </summary>
        /// <returns>The number of empty slots in this Inventory.</returns>
        public virtual int GetEmptySlotCount()
        {
            int emptyCount = 0;
            foreach (RootItemStack x in Content)
            {
                if (x == null) emptyCount++;
            }
            return emptyCount;
        }



        // Client Requests
        /// <summary>
        /// Client requests to move an item in this inventory from slot A to B.
        /// </summary>
        /// <param name="originObjNetId">The NetId of the originating Inventory</param>
        /// <param name="goalObjNetId">The NetId of the target/goal Inventory</param>
        /// <param name="fromSlotIndex">The index of the item in the originating inventory</param>
        /// <param name="toSlotIndex">The index of the item in the target/goal inventory</param>
        /// <param name="originBehaviorIndex">The index of the Behavior of the originating Inventory within its owning NetworkIdentity's NetworkBehaviours list.</param>
        /// <param name="goalBehaviorIndex">The index of the Behavior of the target/goal Inventory within its owning NetworkIdentity's NetworkBehaviours list.</param>
        [Command]
        public virtual void CmdRequestMove(uint originObjNetId, uint goalObjNetId, int fromSlotIndex, int toSlotIndex, int originBehaviorIndex, int goalBehaviorIndex)
        {
            // Debug.Log($"<color=red>SERVER Origin is index {originBehaviorIndex}, goal is index {goalBehaviorIndex}</color> and action is Move");
            if (!IsInitialized) return;

            Inventory originInventory = (Inventory)NetworkPipeline.GetNetworkBehaviour(originObjNetId, originBehaviorIndex, this);
            Inventory goalInventory = (Inventory)NetworkPipeline.GetNetworkBehaviour(goalObjNetId, goalBehaviorIndex, this);

            DoMove(originInventory, goalInventory, fromSlotIndex, toSlotIndex);
        }

        /// <summary>
        /// Send a specific slot update to the client.
        /// </summary>
        /// <param name="index">Which index the client wants an update on.</param>
        [Command]
        public virtual void CmdRefreshSlotFromServer(int index)
        {
            int dbKey;
            int size;

            if (Content[index] != null)
            {
                dbKey = Content[index].Source.GetDbKey();
                size = Content[index].StackSize;
            }
            else
            {
                dbKey = -1;
                size = 0;
            }

            RpcRemoteUpdateSlot(index, dbKey, size, false);
        }

        /// <summary>
        /// Send the entire inventory to the client. Does not require authority, could be and expoitable thing if contents are considered private/sensitive.
        /// </summary>
#if MIRROR && !FISHNET
        [Command(requiresAuthority = false)]
#elif FISHNET
        [ServerRpc(RequireOwnership = false)]
#endif
        public virtual void CmdRefreshAllFromServer()
        {
            // Debug.Log($"<color=orange>||||||||||||||||||||||| Client Requested Full Inventory Refresh ||||||||||||||||||||||| </color>");
            if (!IsInitialized) return;

            for (int i = 0; i < Content.Count; i++)
            {
                int dbKey;
                int size;

                if (Content[i] != null && Content[i].Source != null)
                {
                    dbKey = Content[i].Source.GetDbKey();
                    size = Content[i].StackSize;
                }
                else
                {
                    dbKey = -1;
                    size = 0;
                }

                // Debug.Log($"<color=orange>... SERVER sending update for slot {i}, item id {vaultIndex}, count {size}</color>");
                RpcRemoteUpdateSlot(i, dbKey, size, false);
            }
        }

        /// <summary>
        /// Client requests to split a stack of items.
        /// </summary>
        /// <param name="index">The index of the item in this inventory.</param>
        [Command]
        public virtual void CmdRequestSplit(int index)
        {
            DoSplit(index);
        }

        /// <summary>
        /// Client requests the item stack at index be dropped into the world.
        /// </summary>
        /// <param name="index">The item index in this inventory.</param>
        [Command]
        public virtual void CmdRequestDrop(int index)
        {
            DoDiscard(index);
        }

        /// <summary>
        /// Client requests the item be used.
        /// </summary>
        /// <param name="index">The item index in this inventory.</param>
        [Command]
        public virtual void CmdRequestUse(int index)
        {
            if (Contains(Content[index].Source, 1)) DoUse(index);
        }



        // Server Processes
        /// <summary>
        /// SILENTLY replaces a content index to the provided RootItemStack. Generally not used, other Do[xyz] methods should be used. Only use if you know exactly why you should be setting the item silently.
        /// </summary>
        /// <param name="index">The content index to set</param>
        /// <param name="item">The RootItemStack to put in the index</param>
        [Server]
        public virtual void Set(int index, RootItemStack item)
        {
            Content[index] = item;
        }

        /// <summary>
        /// Immediately add an item to this inventory, if possible.
        /// </summary>
        /// <param name="item">What item to add.</param>
        /// <param name="tryToMerge">Try to merge into existing inventory stacks?</param>
        /// <returns>The amount that could *not* be added. A return of 0 is total success, the amount was fully added.</returns>
        [Server]
        public virtual int DoAdd(RootItemStack item, bool tryToMerge = true)
        {
            if (!IsInitialized)
            {
                Debug.LogWarning("Cannot Add to an uninitialized inventory.", this);
                return -1;
            }
            if (item == null || item.Source == null)
            {
                return -1;
            }
            if (item.StackSize == 0) item.StackSize = 1;

            int hotStack = item.StackSize;

            #region Merge Path
            if (tryToMerge && item.Source.MaxStackSize > 1)
            {
                bool merging = true;
                while (merging)
                {
                    if (hotStack <= 0) return 0;

                    // Try to find a suitable slot to merge into, exit this loop if we can't.
                    int mergeIndex = GetIndexOfItemNotFull(item.Source.GetDbKey());
                    if (mergeIndex == -1)
                    {
                        merging = false;
                        continue;
                    }

                    // If found a matching slot, see how much doesn't fit.
                    int remain = GetMergeRemainder(
                        hotStack,
                        Content[mergeIndex].StackSize,
                        Content[mergeIndex].Source.MaxStackSize);

                    // stash old data, for use as a delta.
                    RootItemStack delta = new RootItemStack(Content[mergeIndex].Source, Content[mergeIndex].StackSize);

                    // Modify the data on the server
                    if (remain > 0) Content[mergeIndex].StackSize = Content[mergeIndex].Source.MaxStackSize;
                    else Content[mergeIndex].StackSize += hotStack;

                    // update the delta amount
                    delta.StackSize = Content[mergeIndex].StackSize - delta.StackSize;

                    // Update live stack count and flag a change on this index.
                    hotStack = remain;

                    if (Application.isBatchMode) OnChanged?.Invoke(mergeIndex);

                    // broadcast add.
                    OnItemAdded?.Invoke(delta);
                    RpcRemoteUpdateSlot(mergeIndex, Content[mergeIndex].Source.GetDbKey(), Content[mergeIndex].StackSize, true);
                }
            }
            #endregion

            #region Open Slot Path
            while (true)
            {
                // Find the first open slot. If there isn't one, or there's no amount left, we're done.
                int openSlot = GetValidNullIndex(item.Source.Restriction);
                if (openSlot == -1 || hotStack <= 0) return hotStack;

                // Otherwise we can add a fresh new item to the list.
                Content[openSlot] = item;
                Content[openSlot].StackSize = 0;

                // And figure out if it fits. (duh, it does, but whatever, maybe StackSize > MaxStackSize because reasons?)
                int remain = GetMergeRemainder(
                    hotStack,
                    Content[openSlot].StackSize,
                    Content[openSlot].Source.MaxStackSize);

                // Then fill up the open slot
                Content[openSlot].StackSize = remain > 0
                    ? Content[openSlot].Source.MaxStackSize
                    : hotStack;

                // And update the hot stack, flag a change and try another loop
                hotStack = remain;
                //OnChanged?.Invoke(openSlot);
                OnItemAdded?.Invoke(Content[openSlot]);
                RpcRemoteUpdateSlot(openSlot, item.Source.GetDbKey(), Content[openSlot].StackSize, true);
            }

            #endregion
        }

        /// <summary>
        /// Immediately move an item from one slot to another slot, even between <see cref="Inventory"/> classes.
        /// </summary>
        /// <param name="origin">Origin inventory, where the item is coming from.</param>
        /// <param name="goal">Goal/target inventory the item will be moved to.</param>
        /// <param name="fromIndex">The item's index in the Origin Inventory.</param>
        /// <param name="toIndex">The target index in the Goal Inventory</param>
        /// <returns>Any remainder count of items that could *not* be moved for some reason. Zero is flawless victory.</returns>
        [Server]
        public virtual int DoMove(Inventory origin, Inventory goal, int fromIndex, int toIndex)
        {
            if (!IsInitialized) return -1;
            if (origin == goal && fromIndex == toIndex) return -1;
            if (origin.Content[fromIndex] == null) return -1;

            // try merge - is goal empty?
            if (goal.Content[toIndex] != null)
            {
                // not empty apparently, so are they different items? if so we can try to swap them.
                if (origin.Content[fromIndex].Source != goal.Content[toIndex].Source)
                {
                    // is the goal slot restriction None or Same as the origin item going there?
                    if (goal.Restrictions[toIndex] != null &&
                        goal.Restrictions[toIndex] != origin.Content[fromIndex].Source.Restriction)
                    {
                        return -1;
                    }

                    // is the origin slot restriction None or Same as goal item going there?
                    if (origin.Restrictions[fromIndex] != null &&
                        origin.Restrictions[fromIndex] != goal.Content[toIndex].Source.Restriction)
                    {
                        return -1;
                    }

                    RootItemStack originCache = origin.Content[fromIndex];
                    RootItemStack goalCache = goal.Content[toIndex];

                    goal.Content[toIndex] = originCache;
                    origin.Content[fromIndex] = goalCache;

                    // for client
                    goal.RpcRemoteUpdateSlot(toIndex, goal.Content[toIndex].Source.GetDbKey(), goal.Content[toIndex].StackSize, false);
                    origin.RpcRemoteUpdateSlot(fromIndex, origin.Content[fromIndex].Source.GetDbKey(), origin.Content[fromIndex].StackSize, false);

                    return 0;
                }

                // alright, apparently not empty - but they're the same type! sooo see how many are left if origin stack merged into goal stack
                int remain = GetMergeRemainder(
                    origin.Content[fromIndex].StackSize,
                    goal.Content[toIndex].StackSize,
                    goal.Content[toIndex].Source.MaxStackSize);

                // they're the same type, so simply set the stack sizes on both slots
                if (remain <= 0)
                {
                    // if theres none remaining, the merge was 100% success
                    goal.Content[toIndex].StackSize += origin.Content[fromIndex].StackSize;
                    origin.Content[fromIndex] = null;

                    goal.RpcRemoteUpdateSlot(toIndex, goal.Content[toIndex].Source.GetDbKey(), goal.Content[toIndex].StackSize, false);
                    origin.RpcRemoteUpdateSlot(fromIndex, -1, 0, false);

                    return 0;
                }

                // otherwise, there is a remainder so max out the goal stack and set the origin stack size to the remainder.
                goal.Content[toIndex].StackSize = goal.Content[toIndex].Source.MaxStackSize;
                origin.Content[fromIndex].StackSize = remain;

                // for client
                goal.RpcRemoteUpdateSlot(toIndex, goal.Content[toIndex].Source.GetDbKey(), goal.Content[toIndex].StackSize, false);
                origin.RpcRemoteUpdateSlot(fromIndex, origin.Content[fromIndex].Source.GetDbKey(), origin.Content[fromIndex].StackSize, false);

                return remain;
            }

            // otherwise, direct move to empty cell.
            // does the slot accept this property?
            if (goal.Restrictions[toIndex] == null || goal.Restrictions[toIndex] == origin.Content[fromIndex].Source.Restriction)
            {
                goal.Content[toIndex] = origin.Content[fromIndex];
                origin.Content[fromIndex] = null;

                // for server
                //origin.OnChanged?.Invoke(fromIndex);
                //goal.OnChanged?.Invoke(toIndex);

                // for client
                goal.RpcRemoteUpdateSlot(toIndex, goal.Content[toIndex].Source.GetDbKey(), goal.Content[toIndex].StackSize, false);
                origin.RpcRemoteUpdateSlot(fromIndex, -1, 0, false);
                return 0;
            }

            return origin.Content[fromIndex].StackSize;
        }

        /// <summary>
        /// Immediately remove, destroy and obliterate an item from this <see cref="Inventory"/>.
        /// </summary>
        /// <param name="index">The index you want to remove, destroy and obliterate.</param>
        /// <returns>Any remainder, but always zero. This can't fail unless the index is out of range.</returns>
        [Server]
        public virtual int DoErase(int index)
        {
            if (!IsInitialized) return -1;

            RootItemStack data = Content[index];
            Content[index] = null;
            OnChanged?.Invoke(index);
            OnItemRemoved?.Invoke(data);
            RpcRemoteUpdateSlot(index, -1, 0, false);
            return 0;
        }

        /// <summary>
        /// Immediately finds item's matching the title and removes an amount. Uses string comparisons. Will scan entire inventory first.
        /// </summary>
        /// <param name="item">The title of the item.</param>
        /// <param name="amountToRemove">The stack size or amount/number of items to remove.</param>
        /// <returns>Any amount that could *not* be removed. Zero is flawless victory. Negative one is error.</returns>
        [Server]
        public virtual int DoTake(RootItem item, int amountToRemove)
        {
            if (!IsInitialized) return -1;

            //Debug.Log($"<color=red>Taking {amountToRemove} {itemTitle}</color>");
            int removedSoFar = 0;
            int[] contentIndexes = GetAllIndexOfItem(item.GetDbKey());
            foreach (int index in contentIndexes)
            {
                if (removedSoFar >= amountToRemove) break;
                int amountRemainingToRemove = amountToRemove - removedSoFar;

                // if the stack has enough for the rest...
                if (Content[index].StackSize > amountRemainingToRemove)
                {
                    Content[index].StackSize -= amountRemainingToRemove;
                    OnChanged?.Invoke(index);
                    OnItemRemoved?.Invoke(new RootItemStack(Content[index].Source, amountRemainingToRemove));
                    RpcRemoteUpdateSlot(index, item.GetDbKey(), Content[index].StackSize, false);
                    removedSoFar = amountToRemove;
                    break;
                }

                // otherwise, nuke that slot.
                removedSoFar += Content[index].StackSize;
                DoErase(index);
            }

            return amountToRemove - removedSoFar;
        }

        /// <summary>
        /// <para>Immediately removes an amount from a specific index in the Inventory. Useful for things like 'hotbar items', or reducing specific indexes after use/consumption.</para>
        /// <para>Uses a string lookup to get the Vault Index for the item.</para>
        /// </summary>
        /// <param name="inventoryIndex">The index of the item stack in the Inventory.</param>
        /// <param name="amountToRemove">How many of the stack to remove.</param>
        /// <returns>Any amount that could *not* be removed. Zero is flawless victory. Negative one is an error. Anything else means failure to Take.</returns>
        public virtual int DoTake(int inventoryIndex, int amountToRemove)
        {
            if (!IsInitialized) return -1;

            // if the amount is exactly the stack size, we should just Erase the content instead.
            if (amountToRemove == Content[inventoryIndex].StackSize)
            {
                DoErase(inventoryIndex);
                return 0;
            }

            // if the amount is less than the stack size, reduce the stack size.
            if (amountToRemove < Content[inventoryIndex].StackSize)
            {
                int itemDbKey = Content[inventoryIndex].Source.GetDbKey();

                Content[inventoryIndex].StackSize -= amountToRemove;
                OnChanged?.Invoke(inventoryIndex);
                OnItemRemoved?.Invoke(new RootItemStack(Content[inventoryIndex].Source, amountToRemove));
                RpcRemoteUpdateSlot(inventoryIndex, itemDbKey, Content[inventoryIndex].StackSize, false);
                return 0;
            }

            // in any other case, we can't *completely* fulfill the request, so we should return the full request amount back. (indicate failure)
            // it's not an error, so doing this seems like the logcal path for this operation.
            // the calling method should verify that all the of the content was taken (returned zero).
            return amountToRemove;
        }

        /// <summary>
        /// Immediately count the number of a specific item in this <see cref="Inventory"/>.
        /// </summary>
        /// <param name="itemDbKey">The DB Key to look for</param>
        /// <returns>The combined stack size total of all items of the given name in the <see cref="Inventory"/>.</returns>
        [Server]
        public virtual int DoCount(int itemDbKey)
        {
            if (!IsInitialized) return -1;

            return GetItemCount(itemDbKey);
        }

        /// <summary>
        /// Immediately discard, or 'drop' an item by spawning it's Art Prefab and removing it's data from the <see cref="Inventory"/> class.
        /// </summary>
        /// <param name="index">Which index in this inventory to discard.</param>
        /// <returns>The spawned RuntimeItemProxy component</returns>
        [Server]
        public virtual RuntimeItemProxy DoDiscard(int index)
        {
            if (!IsInitialized) return null;

            // spawn new
            RuntimeItemProxy result = VaultInventory.SpawnWorldItem(Content[index].Source, InventoryOwner.MyTransform.position, Content[index].StackSize);

            // kill, flag and return.
            Content[index] = null;
            if (Application.isBatchMode) OnChanged?.Invoke(index);
            OnItemRemoved?.Invoke(result.Data);
            RpcRemoteUpdateSlot(index, -1, 0, true);
            return result;
        }

        /// <summary>
        /// Immediately split a stack in half. One empty slot is required. TODO rounding.
        /// </summary>
        /// <param name="index">Index of this <see cref="Content"/> to split.</param>
        /// <returns>TRUE if successfully split and created a new item with half the stack size, FALSE if failure and no action was taken.</returns>
        [Server]
        public virtual bool DoSplit(int index)
        {
            if (!IsInitialized) return false;

            // Is there room?
            int openSlot = GetValidNullIndex(Content[index].Source.Restriction);
            if (openSlot == -1)
            {
                Debug.LogError("Inventory has no space to split stack");
                return false;
            }

            // Figure out the new slot size.
            int newSlotSize = Content[index].StackSize / 2;

            // Try adding it.
            int oudex = DoAdd(new RootItemStack(Content[index].Source, newSlotSize), false);
            if (oudex == -1) return false;

            // Success, so subtract the new slot amount from the original stack.
            Content[index].StackSize -= newSlotSize;

            //OnChanged?.Invoke(index);

            // RPC update (only old slot, new slot was RPC'd via DoAdd() method)
            RpcRemoteUpdateSlot(index, Content[index].Source.GetDbKey(), Content[index].StackSize, false);
            return true;
        }

        /// <summary>
        /// Immediately use an item.
        /// </summary>
        /// <param name="index">Index of this <see cref="Content"/> to Use.</param>
        /// <returns>TRUE if successfully used. False otherwise.</returns>
        [Server]
        public virtual bool DoUse(int index)
        {
            if (!IsInitialized) return false;

            IUseableDataEntity useable = (IUseableDataEntity)Content[index].Source;
            if (useable == null) return false;

            useable.UseBegin(InventoryOwner);

            if (Application.isBatchMode) OnChanged?.Invoke(index);
            return true;
        }



        // Server RPC completions. Every client **including the server client (host)** gets this call. If you're trying to run headless pure server, uncomment the OnChanged code in the [Server] methods.
        [ClientRpc]
        public virtual void RpcRemoteUpdateSlot(int index, int itemDbKey, int amount, bool flagAddRemove)
        {
            //Debug.Log($"<color=green>Inventory || Received RpcUpdateSlot({index}, {itemId}, {amount}) for {gameObject.name}</color>", this);


            // is the new data 'nothing'?
            if (amount == 0 || itemDbKey == -1)
            {
                // we should flag a removal has been made.
                if (!this.IsServer() && Content[index] != null && flagAddRemove)
                {
                    //Debug.Log($"<color=green>Inventory || ... Delete {Content[index].StackSize} of {Content[index].Source.Title}</color>", this);
                    OnItemRemoved?.Invoke(Content[index]);
                }
                Content[index] = null;
                OnChanged?.Invoke(index);
                return;
            }



            // if not then it must be an increase, decrease or a new item.
            RootItemStack newData = new RootItemStack((RootItem)Vault.Get(itemDbKey), amount);
            RootItemStack oldData = Content[index] == null || Content[index].Source == null
                ? null
                : new RootItemStack(Content[index].Source, Content[index].StackSize);


            // is the slot currently holding something?
            if (oldData != null && oldData.Source != null)
            {
                // slot had something so it must be increase or decrease
                if (newData.StackSize < oldData.StackSize)
                {
                    // decreased
                    //Debug.Log($"<color=green>Inventory || ... Decrease {Content[index].StackSize} of {Content[index].Source.Title}</color>", this);

                    Content[index] = newData;
                    Content[index].StackSize = amount;

                    int delta = oldData.StackSize - newData.StackSize;
                    if (!this.IsServer() && flagAddRemove) OnItemRemoved?.Invoke(new RootItemStack(oldData.Source, delta));
                }
                else
                {
                    // increased
                    //Debug.Log($"<color=green>Inventory || ... Increase {newData.StackSize} of {newData.Source.Title}</color>", this);

                    Content[index] = newData;
                    Content[index].StackSize = amount;

                    int delta = newData.StackSize - oldData.StackSize;
                    if (!this.IsServer() && flagAddRemove) OnItemAdded?.Invoke(new RootItemStack(oldData.Source, delta));
                }
            }
            else
            {
                // new addition
                //Debug.Log($"<color=green>Inventory || ... New Item {newData.StackSize} of {newData.Source.Title}</color>", this);

                Content[index] = newData;
                Content[index].StackSize = amount;
                if (flagAddRemove) OnItemAdded?.Invoke(Content[index]);
            }
            OnChanged?.Invoke(index);
        }



        // Internal Gets
        /// <summary>
        /// Looks for the first open slot with the matching restriction and returns it's index.
        /// </summary>
        /// <param name="restriction">The slot restriction.</param>
        /// <returns>The index of the slot found, if any. returning -1 on failure.</returns>
        protected virtual int GetValidNullIndex(SlotRestriction restriction)
        {
            for (int i = 0; i < Content.Count; i++)
            {
                if (Content[i] == null && (Restrictions[i] == restriction || Restrictions[i] == null)) return i;
            }
            return -1;
        }

        /// <summary>
        /// Looks for an item in this inventory by the Item's DB Key.
        /// </summary>
        /// <param name="itemDbKey">The DB Key of the item.</param>
        /// <param name="startIndex">The index to start searching from.</param>
        /// <returns>The index of an item that matches the title parameter</returns>
        protected virtual int GetIndexOfItem(int itemDbKey, int startIndex = 0)
        {
            for (int i = startIndex; i < Content.Count; i++)
            {
                if (Content[i] == null) continue;
                if (Content[i].Source.GetDbKey() == itemDbKey) return i;
            }
            return -1;
        }

        /// <summary>
        /// Find every index that contains the item with a given title.
        /// </summary>
        /// <param name="itemDbKey">The item's Database Key - use RootItem.GetDbKey() to retrieve.</param>
        /// <returns>An array of indexes where this item exists.</returns>
        protected virtual int[] GetAllIndexOfItem(int itemDbKey)
        {
            List<int> t = new List<int>();
            int curIndex = 0;
            while (curIndex < Content.Count)
            {
                if (Content[curIndex] != null && Content[curIndex].Source.GetDbKey() == itemDbKey) t.Add(curIndex);
                curIndex++;
            }

            return t.ToArray();
        }

        /// <summary>
        /// Looks for a slot that contains a specific item and is not full.
        /// </summary>
        /// <param name="itemDbKey">The DB Key of the item.</param>
        /// <param name="startIndex">The index to start searching from.</param>
        /// <returns></returns>
        protected virtual int GetIndexOfItemNotFull(int itemDbKey, int startIndex = 0)
        {
            for (int i = startIndex; i < Content.Count; i++)
            {
                if (Content[i] == null || Content[i].Source == null) continue;
                if (Content[i].Source.GetDbKey() == itemDbKey && Content[i].StackSize < Content[i].Source.MaxStackSize)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Shorthand math method to get the amount of items leftover after merging two stacks.
        /// </summary>
        /// <param name="fromSize">The origin's stack size. Eg `itemBeingMoved.StackSize`</param>
        /// <param name="destinationSize">The destination's current stack size. Eg `someTargetItem.StackSize`</param>
        /// <param name="maxSize">The destination's maximum stack size. Eg `someTargetItem.MaxStackSize`</param>
        /// <returns>How many should remain at the origin because they don't fit in the destination.</returns>
        protected virtual int GetMergeRemainder(int fromSize, int destinationSize, int maxSize)
        {
            int spaceAvailable = maxSize - destinationSize;
            return Mathf.Clamp(fromSize - spaceAvailable, 0, fromSize);
        }

        /// <summary>
        /// Find the total quantity of an item in this Inventory by scanning all of the slots.
        /// </summary>
        /// <param name="itemDbKey">The DB Key of the item you want to find to count.</param>
        /// <returns>The total stack count of the item in the entire inventory.</returns>
        protected virtual int GetItemCount(int itemDbKey)
        {
            int result = 0;
            foreach (RootItemStack x in Content)
            {
                if (x == null) continue;
                if (x.Source.GetDbKey() == itemDbKey)
                {
                    result += x.StackSize;
                }
            }
            return result;
        }

        protected virtual void OnDestroy()
        {
            OnDestroyed?.Invoke(this);
        }
    }
}