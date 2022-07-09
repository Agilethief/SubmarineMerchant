using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using Mirror;

namespace CargoGame
{
    public class SteamLobby : MonoBehaviour
    {
        public static SteamLobby instance;

        // Steam callbacks
        protected Callback<LobbyCreated_t> lobbyCreated;
        protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
        protected Callback<LobbyEnter_t> lobbyEntered;
        protected Callback<LobbyMatchList_t> lobbyMatchList;
        protected Callback<LobbyDataUpdate_t> lobbyDataUpdate;

        // Steam lobby data
        public ulong current_lobbyID;
        public List<CSteamID> lobbyIDS = new List<CSteamID>();  // All the different lobby IDs gathered from steam

        private const string HostAddressKey = "HostAddress";

        public STNetworkManager networkManager;

        // Start is called before the first frame update
        void Start()
        {
            networkManager = GetComponent<STNetworkManager>(); // Grab the network manager that should be attached to this object.

            if (!SteamManager.Initialized) return;

            // Setting up the callbacks.
            lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
            gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
            lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
            lobbyMatchList = Callback<LobbyMatchList_t>.Create(OnGetLobbiesList);
            lobbyDataUpdate = Callback<LobbyDataUpdate_t>.Create(OnGetLobbyInfo);

            // Setup the instance
            if (instance == null)
                instance = this;

        }

        // Call back functions

        // When the lobby is created
        void OnLobbyCreated(LobbyCreated_t callback)
        {
            Debug.Log("Creating lobby");
            // Ensure we have successfully created the lobby with Steam 
            if (callback.m_eResult != EResult.k_EResultOK) return;
            
            // Send us to the gamescene, where the GameStarter.cs will handle starting the client
            MainMenuManager.Instance.OpenScene("GameScene");
            

            // Do these need to be in the GameStarter?
            // With the lobby we just created, populate it with data for Steam to use.
            SteamMatchmaking.SetLobbyData( new CSteamID(callback.m_ulSteamIDLobby),  HostAddressKey,  SteamUser.GetSteamID().ToString());

            // Creating a second set of lobby data? This is the one that holds teh name of the lobby. Is this one needed?
            SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name", SteamFriends.GetPersonaName().ToString() + "'s lobby");

        }


        // When someone attmpts to join
         void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
        {
            //Debug.Log("OnGameLobbyJoinRequested");
            SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
        }

        // When someone enters the lobby - As in, they have clicked the button to join one,
         void OnLobbyEntered(LobbyEnter_t callback)
        {
            Debug.Log("Entering lobby");
            // Set the current ID to the ID of the lobby we have entered
             current_lobbyID = callback.m_ulSteamIDLobby;
            if (NetworkServer.active) {
                Debug.Log("Network server is already active, breaking out of lobby");
                return;   // If the our own network server is active, we are in the wrong place? check this.
            }
            // Get the address from the lobby to use as the network address
            string hostAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby),HostAddressKey);

            Debug.Log("Setting Host address " + hostAddress);
            // Set address and start the client on that network
            networkManager.networkAddress = hostAddress;

            // Send us to the gamescene, where the GameStarter.cs will handle starting the client
            MainMenuManager.Instance.OpenScene("GameScene");

            // Clearing out old lobby data? 
            lobbyIDS.Clear();

        }

        // Gets the list of lobbies
         void OnGetLobbiesList(LobbyMatchList_t result)
        {
            // Tells us how many lobbies were found
             //Debug.Log("Found " + result.m_nLobbiesMatching + " lobbies!");

            // Go through the found lobbies and collect their data. We'll use these when we genereate a GUI lobby list.
            for (int i = 0; i < result.m_nLobbiesMatching; i++)
            {
                CSteamID lobbyID = SteamMatchmaking.GetLobbyByIndex(i);
                lobbyIDS.Add(lobbyID);
                SteamMatchmaking.RequestLobbyData(lobbyID);

            }
        }

        // On getting info about a lobby
         void OnGetLobbyInfo(LobbyDataUpdate_t result)
        {
            // Report this info to the main menu lobby list
            MainMenuManager.Instance.findPage.DisplayLobbies(lobbyIDS, result);
        }


        // Other lobby functions

        // This starts a new steam lobby
        public void CreateNewLobby(ELobbyType lobbyType)
        {
            //networkManager.GetComponent<STNetworkManager>().networkType = STNetworkManager.NetworkType.Host;
            SteamMatchmaking.CreateLobby(lobbyType, networkManager.maxConnections);
        }

        // Use this to connect to a steam lobby via its ID
        public void JoinLobby(CSteamID lobbyId)
        {
            //networkManager.GetComponent<STNetworkManager>().networkType = STNetworkManager.NetworkType.Client;
            //Debug.Log("JoinLobby: Will try to join lobby with steam id: " + lobbyId.ToString());
            SteamMatchmaking.JoinLobby(lobbyId);
        }

        // Use this to get the list of lobbys from steam
        public void GetListOfLobbies()
        {

            if (lobbyIDS.Count > 0)
                lobbyIDS.Clear();

            SteamMatchmaking.AddRequestLobbyListFilterSlotsAvailable(1);

            SteamAPICall_t try_getList = SteamMatchmaking.RequestLobbyList();
        }

    }
}
