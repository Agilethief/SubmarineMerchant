using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace CargoGame
{
    public class GUI_WorldEntry : GUIBehaviour
    {
        public Button self;
        public string worldNameVisible;
        public string worldName;
        public TMP_Text buttonText;

        private void Start()
        {
            self.onClick.AddListener(OpenWorld);

            buttonText.text = worldNameVisible;
        }

        public void OpenWorld()
        {
            // Selected world = worldname
            GameData.SelectedWorldName = worldName;
            if (MainMenuManager.Instance.openWorldPage.onlyServerToggle.isOn)
                GameData.startType = GameData.GameStartType.Dedicated;
            else
                GameData.startType = GameData.GameStartType.Host;

            GameData.hostingGame = true;

            // Start loading world
            MainMenuManager.Instance.OpenScene("GameScene");
        }

    }
}
