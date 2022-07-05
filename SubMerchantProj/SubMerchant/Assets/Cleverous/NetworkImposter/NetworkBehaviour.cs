// (c) Copyright Cleverous 2022. All rights reserved.

#if !MIRROR && !FISHNET
using UnityEngine;

namespace Cleverous.NetworkImposter
{
    [RequireComponent(typeof(NetworkIdentity))]
    public class NetworkBehaviour : MonoBehaviour
    {
        [HideInInspector]
        public bool isServer = true;
        [HideInInspector]
        public bool isClient = true;
        [HideInInspector]
        public bool hasAuthority = true;
        [HideInInspector]
        public bool isLocalPlayer = true;
        [HideInInspector]
        public NetworkIdentity netIdentity;
        public int netId => netIdentity.netId;
        [HideInInspector]
        public NetworkConnection connectionToClient;

        protected virtual void Start()
        {
            OnStartServer();
            OnStartLocalPlayer();
            OnStartClient();
        }

        public virtual void OnStartClient()
        {
        }

        public virtual void OnStopClient()
        {
        }

        public virtual void OnStartServer()
        {
        }

        public virtual void OnStopServer()
        {
        }

        public virtual void OnStartLocalPlayer()
        {
        }
    }
}
#endif