using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Steamworks;
using TMPro;

namespace CargoGame
{
    public class GUI_LobbyItem : MonoBehaviour
    {
        private Button self;

        public CSteamID lobbySteamId;
        public string lobbyName;
        public int numberOfPlayers;
        public int maxNumberOfPlayers;


        [SerializeField] private TMP_Text LobbyNameText;
        [SerializeField] private TMP_Text NumerOfPlayersText;

        private void Start()
        {
            self = GetComponent<Button>();
            self.onClick.AddListener(JoinGameClick);

        }


        public void SetLobbyItemValues()
        {
            LobbyNameText.text = "Join: " + lobbyName;
            NumerOfPlayersText.text = "Players: " + numberOfPlayers.ToString() + "/" + maxNumberOfPlayers.ToString();
        }

        public void JoinGameClick()
        {
            GameData.startType = GameData.GameStartType.Client;
            SteamLobby.instance.JoinLobby(lobbySteamId);
        }

    }
}
