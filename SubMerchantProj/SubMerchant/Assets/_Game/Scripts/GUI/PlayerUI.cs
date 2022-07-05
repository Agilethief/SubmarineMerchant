using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cleverous.VaultInventory;

namespace CargoGame
{
    public class PlayerUI : GUIBehaviour
    {

        public GUI_CrosshairPanel crosshairPanel;

        public SimplePlayer owningPlayer;

        public GameObject inventoryPanel;
        public InventoryUi inventoryUi;
        public GUI_Hotbar hotbarUi;
        public GUI_StatsPanel statsPanel;
        public GUI_Debug debugPanel;

        // Call when the player first creates this UI
        public void SetupUI(SimplePlayer creatingPlayer)
        {
            Debug.Log("Setting up player UI");
            crosshairPanel.RevealCrosshair();
            HideInventory();
            owningPlayer = creatingPlayer;
            debugPanel.player = creatingPlayer;
        }



        public void RevealMouse(bool reveal)
        {
            if (reveal)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

        }

        public void ShowInventory()
        {
            inventoryPanel.SetActive(true);
        }
        public void HideInventory()
        {
            inventoryPanel.SetActive(false);
        }

        public void CloseAllPanels()
        {
           HideInventory();

        }

    }
}
