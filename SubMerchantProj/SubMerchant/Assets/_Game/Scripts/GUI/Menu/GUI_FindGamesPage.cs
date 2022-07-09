using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Steamworks;

namespace CargoGame
{
    public class GUI_FindGamesPage : GUI_MenuPage
    {

        public Transform scrollContainer;

        public GUI_LobbyItem lobbyItemTemplate;
        public Button bRefreshLobby;

        List<GUI_LobbyItem> lobbyEntries = new List<GUI_LobbyItem>();


        public override void Start()
        {
            base.Start();
            bRefreshLobby.onClick.AddListener(RefreshLobbySearch);
        }

        public override void OpenPage()
        {
            base.OpenPage();

            RefreshLobbySearch();
        }

        void RefreshLobbySearch()
        {
            ClearLobbies();
            SteamLobby.instance.GetListOfLobbies();
        }


        void ClearLobbies()
        {
            if(lobbyEntries.Count <= 0) return;

            for(int i = 0; i < lobbyEntries.Count; i++)
            {
                Destroy(lobbyEntries[i].gameObject);
            }

            lobbyEntries.Clear();
        }


        public void DisplayLobbies(List<CSteamID> lobbyIDS, LobbyDataUpdate_t result)
        {
            for (int i = 0; i < lobbyIDS.Count; i++)
            {
                if (lobbyIDS[i].m_SteamID == result.m_ulSteamIDLobby)
                {
                    Debug.Log("Lobby " + i + " :: " + SteamMatchmaking.GetLobbyData((CSteamID)lobbyIDS[i].m_SteamID, "name") + " Players: " + SteamMatchmaking.GetNumLobbyMembers((CSteamID)lobbyIDS[i].m_SteamID).ToString() + " / " + SteamMatchmaking.GetLobbyMemberLimit((CSteamID)lobbyIDS[i].m_SteamID).ToString());

                    Debug.Log("OnGetLobbyInfo: Player searched for lobbies");

                    GUI_LobbyItem newLobbyListItem = Instantiate(lobbyItemTemplate);
                    newLobbyListItem.gameObject.SetActive(true);

                    // Load up the lobby item with all the values it needs.
                    newLobbyListItem.lobbySteamId = (CSteamID)lobbyIDS[i].m_SteamID;
                    newLobbyListItem.lobbyName = SteamMatchmaking.GetLobbyData((CSteamID)lobbyIDS[i].m_SteamID, "name");
                    newLobbyListItem.numberOfPlayers = SteamMatchmaking.GetNumLobbyMembers((CSteamID)lobbyIDS[i].m_SteamID);
                    newLobbyListItem.maxNumberOfPlayers = SteamMatchmaking.GetLobbyMemberLimit((CSteamID)lobbyIDS[i].m_SteamID);
                    newLobbyListItem.SetLobbyItemValues();


                    newLobbyListItem.transform.SetParent(scrollContainer);
                    newLobbyListItem.transform.localScale = Vector3.one;

                    lobbyEntries.Add(newLobbyListItem);

                    return;
                }
            }

        }

    }
}
