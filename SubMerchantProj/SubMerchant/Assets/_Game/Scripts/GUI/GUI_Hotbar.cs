using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cleverous.VaultInventory;
using Cleverous.VaultSystem;

namespace CargoGame
{
    public class GUI_Hotbar : GUIBehaviour
    {
        [SerializeField] private HotbarUiPanel hotbarPanel;

        public RectTransform selectedSlotImage;
        int hotbarIndex;

        DataEntity selectedDataEntity;
        RootItem selectedRootItem;

        public void SetSelectedHotbarItem(int slotIndex)
        {
            selectedSlotImage.position = hotbarPanel.HotbarSlots[slotIndex].transform.position;

            // Attempt equip hotbar item
            
            if(hotbarPanel.HotbarSlots[slotIndex].TargetData == null) return; // Make sure there is something in the slot

            selectedDataEntity = Vault.Get(hotbarPanel.HotbarSlots[slotIndex].TargetData.GetDbKey());
            selectedRootItem = (RootItem)selectedDataEntity;

            if (selectedRootItem == null) return; // Ensure we have an item in the slot
            if(selectedRootItem.artPrefabHeld == null) return; // Ensure there is an item that can be spawned

            SimplePlayer.localPlayerObject.EquipItem(selectedRootItem.artPrefabHeld);
            

        }

        public void IncrementHotbar()
        {
            hotbarIndex++;

           if(hotbarIndex >= hotbarPanel.HotbarSlots.Count) hotbarIndex = 0;

            SetSelectedHotbarItem(hotbarIndex);
        }
        public void DecrementHotbar()
        {
            hotbarIndex--;

            if(hotbarIndex < 0) hotbarIndex = hotbarPanel.HotbarSlots.Count-1;

            SetSelectedHotbarItem(hotbarIndex);
        }

        public void UseSelectedHotbarItem()
        {
            hotbarPanel.HotbarSlots[hotbarIndex].Interact();
        }

    }
}
