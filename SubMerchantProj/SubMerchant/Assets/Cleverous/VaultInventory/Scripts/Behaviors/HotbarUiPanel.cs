// (c) Copyright Cleverous 2020. All rights reserved.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cleverous.VaultInventory
{
    public class HotbarUiPanel : MonoBehaviour
    {
        public List<HotbarUiPlug> HotbarSlots;
        public GameObject PanelWrapper;

        /// <summary>
        /// Is the Hotbar locked? Locked Hotbar cannot be modified, but slotted entities can be interacted with.
        /// </summary>
        public bool IsLocked { get; protected set; }
        /// <summary>
        /// Is the Hotbar Menu open/visible?
        /// </summary>
        public bool IsOpen { get; protected set; }
        /// <summary>
        /// When the Hotbar is shown/opened, this will fire.
        /// </summary>
        public Action OnOpened;
        /// <summary>
        /// When the Hotbar is hidden/closed, this will fire.
        /// </summary>
        public Action OnClosed;

        /// <summary>
        /// Stores a list of Vault Data Indexes that indicate what entities are slotted in each hotbar slot.
        /// </summary>
        protected List<int> RuntimeIndexAssignments;

        /// <summary>
        /// The owner of this Hotbar.
        /// </summary>
        public IUseInventory Owner;

        protected virtual void Awake()
        {
            RuntimeIndexAssignments = new List<int>();
            VaultInventory.OnPlayerSpawn += SetOwner;
            for (int i = 0; i < HotbarSlots.Count; i++)
            {
                HotbarSlots[i].OnChanged += SlotWasModified;
                HotbarSlots[i].SetDockId(i);
            }

            Show();
        }
        /// <summary>
        /// Initialize the Hotbar. Pulls data from the internal list of assignments. Extend class to add functionality to restore a state by setting that list.
        /// </summary>
        /// <param name="vaultDataIndexes"></param>
        public virtual void Initialize(List<int> vaultDataIndexes)
        {
            for (int i = 0; i < vaultDataIndexes.Count; i++)
            {
                AssignHotbarReference(i, vaultDataIndexes[i]);
            }
        }
        /// <summary>
        /// Assign a specific slot to be a specific entity in the Vault. Can be used for serialization/preference restore purposes.
        /// </summary>
        /// <param name="hotbarIndex">The index of the Hotbar you want to assign.</param>
        /// <param name="vaultDataIndex">The Vault Data Index of the DataEntity you want to slot in.</param>
        public virtual void AssignHotbarReference(int hotbarIndex, int vaultDataIndex)
        {
            RuntimeIndexAssignments[hotbarIndex] = vaultDataIndex;
        }
        /// <summary>
        /// Set the Owner of the Hotbar. Handled during Initialize() but can be changed later.
        /// </summary>
        /// <param name="inv"></param>
        public virtual void SetOwner(IUseInventory inv)
        {
            Owner = inv;
        }
        /// <summary>
        /// Remotely use a slot. Useful for setting up character inputs to activate specific slot IDs.
        /// </summary>
        /// <param name="index"></param>
        public virtual void ActivateSlotRemotely(int index)
        {
            HotbarSlots[index].Interact();
        }

        /// <summary>
        /// Set the lock state of the Hotbar. Locked Hotbar cannot be modified, but slotted entities can be interacted with.
        /// </summary>
        /// <param name="isLocked"></param>
        public virtual void SetLockState(bool isLocked)
        {
            IsLocked = isLocked;
        }
        /// <summary>
        /// Invert the current lock state of the Hotbar. Locked Hotbar cannot be modified, but slotted entities can be interacted with.
        /// </summary>
        public virtual void ToggleLockState()
        {
            IsLocked = !IsLocked;
        }

        /// <summary>
        /// Internal callback used when any slot changes. Checks against other slots to avoid duplicate object entries.
        /// </summary>
        /// <param name="dockId"></param>
        protected virtual void SlotWasModified(int dockId)
        {
            // TODO since this is a reactive thing there is an exploitable in it.
            // TODO the user can drag the item from the inventory to a different hotbar slot and it will remove cooldown.
            // TODO part of the protection for this is handled by the HandleDragEvent() method.
            for (int i = 0; i < HotbarSlots.Count; i++)
            {
                if (i == dockId) continue;
                if (HotbarSlots[i].TargetData == HotbarSlots[dockId].TargetData)
                {
                    HotbarSlots[i].Clear();
                }
            }
        }

        /// <summary>
        /// Show the Hotbar. This enables the PanelWrapper object.
        /// </summary>
        public virtual void Show()
        {
            PanelWrapper.SetActive(true);
            IsOpen = true;
            OnOpened?.Invoke();
        }
        /// <summary>
        /// Hide the Hotbar. This disables the PanelWrapper object.
        /// </summary>
        public virtual void Hide()
        {
            PanelWrapper.SetActive(false);
            IsOpen = false;
            OnClosed?.Invoke();
        }
    }
}