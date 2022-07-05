using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace CargoGame
{
    public class STNetworkManager : NetworkManager
    {
        public override void Awake()
        {
            base.Awake();
        }

        public override void OnStartServer()
        {
            base.OnStartServer();

            NetworkServer.RegisterHandler<PlayerInfoMessage>(OnCreateCharacter);
        }

        // This is called on the server as a new connection is made.
        public override void OnServerConnect(NetworkConnectionToClient conn)
        {
            base.OnServerConnect(conn);

            // Send the new client the world data we have.
            

        }
        public override void OnClientConnect()
        {
            base.OnClientConnect();
            PlayerInfoMessage playerInfoMessage = new PlayerInfoMessage();
            playerInfoMessage.playerName = "NewPlayerName";

            NetworkClient.Send(playerInfoMessage);
        }

        public override void OnClientDisconnect()
        {
            base.OnClientDisconnect();

        }

        void OnCreateCharacter(NetworkConnectionToClient conn, PlayerInfoMessage message)
        {
            // Here we spawn the playerobject and hook them up as the controller

            GameObject character = Instantiate(playerPrefab);
            NetworkServer.AddPlayerForConnection(conn, character);
        }



    }


    struct PlayerInfoMessage : NetworkMessage
    {
        public string playerName;
    }
}