// (c) Copyright Cleverous 2020. All rights reserved.

#if MIRROR && !FISHNET
using Mirror;
#elif FISHNET
using FishNet.Managing;
#endif

using UnityEngine;
using Cleverous.NetworkImposter;

namespace Cleverous.VaultInventory
{
    public class VaultSingleplayerLoader : MonoBehaviour
    {
        public NetworkManager Manager;
        public bool GameIsSinglePlayerOnly;

        public void Awake()
        {
            if (GameIsSinglePlayerOnly)
            {
#if MIRROR && !FISHNET
                NetworkServer.dontListen = true;
#elif FISHNET
                Manager.TransportManager.Transport.SetMaximumClients(1);
#endif
            }
        }

        public void Start()
        {
            if (!GameIsSinglePlayerOnly) return;

#if MIRROR && !FISHNET
            Manager.networkAddress = "localhost";
            Manager.StartHost();
#elif FISHNET
            Manager.ServerManager.StartConnection();
            Manager.ClientManager.StartConnection("localhost");
#endif
        }
    }
}