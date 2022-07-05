// (c) Copyright Cleverous 2020. All rights reserved.

using System;
using Cleverous.NetworkImposter;
using UnityEngine;
using Object = UnityEngine.Object;

#if MIRROR && !FISHNET
using Mirror;
#elif FISHNET
#endif

namespace Cleverous.VaultInventory
{
    public static class VaultInventory
    {
        public static Canvas GameCanvas;
        
        public static GameObject ItemSlotTemplate;
        public static GameObject ItemFloaterTemplate;
        public static GameObject RuntimeItemTemplate;
        public static GameObject GenericInventoryUi;

        // ########### Static Session Events
        /// <summary>
        /// Should be fired when the player is spawned.
        /// </summary>
        public static Action<IUseInventory> OnPlayerSpawn;
        /// <summary>
        /// Should be fired when the scene changes.
        /// </summary>
        public static Action OnStartSceneChange;
        /// <summary>
        /// Fired by Vault when a UI item begins being lifted.
        /// </summary>
        public static Action<ItemUiPlug> OnMoveItemBegin;
        /// <summary>
        /// Fired by Vault when a lifted UI item is dropped.
        /// </summary>
        public static Action<ItemUiPlug> OnMoveItemEnd;
        // ###########

        /// <summary>
        /// Set runtime references. Required for proper operation.
        /// </summary>
        /// <param name="canvas">The Game Canvas</param>
        /// <param name="itemSlot">A Prefab for the ItemSlot's in the Inventory UI</param>
        /// <param name="itemFloater">A Prefab for the ItemFloater (items being dragged around) in the Inventory UI</param>
        /// <param name="itemRuntime">A Prefab for the Runtime items spawned into the world. Should have a RuntimeItemProxy script on it.</param>
        /// <param name="genericInventory">A Prefab for a generic inventory - used when opening to view contents of other inventories like crates, etc.</param>
        public static void InitReferences(Canvas canvas, GameObject itemSlot, GameObject itemFloater, GameObject itemRuntime, GameObject genericInventory)
        {
            GameCanvas = canvas;
            ItemSlotTemplate = itemSlot;
            ItemFloaterTemplate = itemFloater;
            RuntimeItemTemplate = itemRuntime;
            GenericInventoryUi = genericInventory;
        }

        /// <summary>
        /// Spawns a new item into the world. Creates a wrapper object from the template, spawns the art as a child and assigns the correct properties.
        /// </summary>
        /// <param name="item">The item to spawn.</param>
        /// <param name="pos">Target position to spawn at.</param>
        /// <param name="stackSize">Stack Size of the spawned item.</param>
        /// <returns>Returns the RuntimeItemProxy component on the wrapper if successful.</returns>
        public static RuntimeItemProxy SpawnWorldItem(RootItem item, Vector3 pos, int stackSize)
        {
            if (!NetworkPipeline.StaticIsServer())
            {
                Debug.LogError("Network Server error when trying to spawn runtime items.");
                return null;
            }

            if (item == null || item.artPrefabWorld == null)
            {
                Debug.LogError("Failed SpawnWorldItem(). The input Source item or Art Prefab object was null when trying to SpawnWorldItem().");
                return null;
            }

            GameObject wrapper = Object.Instantiate(RuntimeItemTemplate, pos, Quaternion.identity);
            RuntimeItemProxy itemComponent = wrapper.GetComponent<RuntimeItemProxy>();

            if (itemComponent == null)
            {
                Debug.LogError("Failed SpawnWorldItem(). No RuntimeItemProxy component found. GameObject is floating garbage. You must add a RuntimeItemProxy to the Item Template Prefab.", wrapper);
                return null;
            }

            itemComponent.SvrInitialize(item, stackSize); // sets the syncvar values.
            NetworkPipeline.Spawn(wrapper);
            return itemComponent;
        }

        public static GameObject SpawnInventoryUi(Inventory bindTo, string inventoryName = "Inventory")
        {
            GameObject go = Object.Instantiate(GenericInventoryUi, GameCanvas.transform);
            go.GetComponentInChildren<InventoryUi>().SetTargetInventory(bindTo);
            return go;
        }

        /// <summary>
        /// Give an item to a target inventory.
        /// </summary>
        /// <param name="target">Target Inventory to give the item to</param>
        /// <param name="stack">The item to give</param>
        /// <returns>Any remainder of stack. Zero is complete success. -1 is an error.</returns>
        public static int TryGiveItem(Inventory target, RootItemStack stack)
        {
            if (stack != null && target != null) return target.DoAdd(stack);
            Debug.LogError("Error in TryGiveItem() call.");
            return -1;
        }
    }
}