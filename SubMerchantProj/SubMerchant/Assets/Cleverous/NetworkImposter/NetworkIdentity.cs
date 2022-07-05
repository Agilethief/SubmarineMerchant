// (c) Copyright Cleverous 2022. All rights reserved.

#if !MIRROR && !FISHNET
using System.Collections.Generic;
using UnityEngine;

namespace Cleverous.NetworkImposter
{
    public class NetworkIdentity : MonoBehaviour
    {
        public static Dictionary<int, NetworkIdentity> allNetworkIdentities;
        [HideInInspector]
        public int netId;
        [HideInInspector]
        public NetworkConnection connectionToClient;
        [HideInInspector]
        public NetworkBehaviour[] Behaviours;

        private static int idCounter;

        private void Awake()
        {
            if (allNetworkIdentities == null) allNetworkIdentities = new Dictionary<int, NetworkIdentity>();

            Behaviours = GetComponents<NetworkBehaviour>();
            for (int i = 0; i < Behaviours.Length; i++)
            {
                Behaviours[i].netIdentity = this;
            }

            netId = NextObjectId();
            allNetworkIdentities.Add(netId, this);
        }

        private void OnDestroy()
        {
            allNetworkIdentities.Remove(netId);
        }

        public void AssignClientAuthority(NetworkConnection conn)
        {

        }

        public void RemoveClientAuthority()
        {

        }

        public static int NextObjectId()
        {
            return idCounter++;
        }
    }
}
#endif