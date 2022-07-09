using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace CargoGame
{

    // This script connects the player to their designated server from the menu
    public class GameStarter : BaseClientOnlyBehaviour
    {

        STNetworkManager netManager { get { return NetworkManager.singleton.GetComponent<STNetworkManager>(); } }

        // Start is called before the first frame update
        IEnumerator Start()
        {
            yield return null;
            yield return null;

            Debug.Log( GameData.startType.ToString());
            switch (GameData.startType)
            {
                case  GameData.GameStartType.Client:
                    StartClient();
                    break;
                case GameData.GameStartType.Host:
                    StartHost();
                    break;
                case GameData.GameStartType.Dedicated:
                    StartServer();
                    break;
            }

            yield return null;
        }

        void StartClient()
        {
            netManager.StartClient();
        }

        void StartHost()
        {
            // The Mirror to START the game as a HOST.
            netManager.StartHost();
        }

        void StartServer()
        {

        }

    }
}
