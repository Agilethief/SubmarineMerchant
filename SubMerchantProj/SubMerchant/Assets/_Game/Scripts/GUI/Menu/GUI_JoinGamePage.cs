using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace CargoGame
{
    public class GUI_JoinGamePage : GUI_MenuPage
    {
        public TMP_InputField ipInputfield;

        public Button joinButton;


                public override void Start()
        {
            base.Start();

            joinButton.onClick.AddListener(JoinWorld);
        }


        void JoinWorld()
        {
            if(ipInputfield.text.Length < 1)
                return;

            GameData.ipAddressToConnect = ipInputfield.text;
            GameData.startType = GameData.GameStartType.Client;
            GameData.hostingGame = false;

            MainMenuManager.Instance.OpenScene("GameScene");

        }

    }
}
