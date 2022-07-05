// (c) Copyright Cleverous 2022. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if MIRROR && !FISHNET
using Mirror;
#elif FISHNET
using FishNet;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Managing; // leave this here
using NetworkIdentity = FishNet.Object.NetworkObject;
#endif

namespace Cleverous.NetworkImposter
{
    public static class NetworkPipeline
    {
        public static uint NetId(this NetworkIdentity ident)
        {
#if MIRROR && !FISHNET
            return ident.netId;
#elif FISHNET
            return (uint)ident.ObjectId;
#else
            return (uint) ident.netId;
#endif
        }
        public static uint NetId(this NetworkBehaviour nb)
        {
#if MIRROR && !FISHNET
            return nb.netId;
#elif FISHNET
            return (uint)nb.ObjectId;
#else 
            return (uint)nb.netId;
#endif
        }
        public static NetworkIdentity NetIdentity(this NetworkBehaviour nb)
        {
#if MIRROR && !FISHNET || !MIRROR && !FISHNET
            return nb.netIdentity;
#elif FISHNET
            return nb.NetworkObject;
#endif
        }

        public static NetworkConnection Owner(this NetworkBehaviour nb)
        {
#if MIRROR && !FISHNET || !MIRROR && !FISHNET
            return nb.connectionToClient;
#elif FISHNET
            return nb.Owner;
#endif
        }
        public static NetworkConnection Owner(this NetworkIdentity ni)
        {
#if MIRROR && !FISHNET || !MIRROR && !FISHNET
            return ni.connectionToClient;
#elif FISHNET
            return ni.Owner;
#endif
        }

        public static NetworkIdentity GetNetworkIdentity(uint objectId, NetworkBehaviour nb = null)
        {
#if MIRROR && !FISHNET
            return NetworkServer.active 
                ? NetworkServer.spawned[objectId] 
                : NetworkClient.spawned[objectId];
#elif FISHNET
            Dictionary<int, NetworkIdentity> dict = nb == null 
                ? InstanceFinder.ServerManager.Objects.Spawned 
                : nb.NetworkManager.ServerManager.Objects.Spawned;
            return dict[(int)objectId];
#else
            return NetworkIdentity.allNetworkIdentities[(int)objectId];
#endif
        }
        public static NetworkBehaviour GetNetworkBehaviour(uint objectId, int nbId, NetworkBehaviour nb = null)
        {
#if MIRROR && !FISHNET
            return NetworkServer.active 
                ? NetworkServer.spawned[objectId].NetworkBehaviours[nbId] 
                : NetworkClient.spawned[objectId].NetworkBehaviours[nbId];
#elif FISHNET
            Dictionary<int, NetworkIdentity> dict = nb == null 
                ? InstanceFinder.ServerManager.Objects.Spawned 
                : nb.NetworkManager.ServerManager.Objects.Spawned;
            return dict[(int)objectId].NetworkBehaviours[nbId];
#else
            return NetworkIdentity.allNetworkIdentities[(int)objectId].Behaviours[nbId];
#endif
        }
        public static NetworkBehaviour[] GetNetworkBehaviours(this NetworkBehaviour nb)
        {
#if MIRROR && !FISHNET
            return nb.netIdentity.NetworkBehaviours;
#elif FISHNET
            return nb.NetworkObject.NetworkBehaviours;
#else
            return nb.netIdentity.Behaviours;
#endif
        }
        public static NetworkBehaviour[] GetNetworkBehaviours(this NetworkIdentity netIdentity)
        {
#if MIRROR && !FISHNET
            return netIdentity.NetworkBehaviours;
#elif FISHNET
            return netIdentity.NetworkBehaviours;
#else
            return netIdentity.Behaviours;
#endif
        }

        public static bool StaticIsServer()
        {
#if MIRROR && !FISHNET || !MIRROR && !FISHNET
            return NetworkServer.active;
#elif FISHNET
            return InstanceFinder.IsServer;
#endif
        }
        public static bool StaticIsClient()
        {
#if MIRROR && !FISHNET || !MIRROR && !FISHNET
            return NetworkClient.active;
#elif FISHNET
            return InstanceFinder.IsClient;
#endif
        }

        public static bool IsServer(this NetworkBehaviour nb)
        {
#if MIRROR && !FISHNET || !MIRROR && !FISHNET
            return nb.isServer;
#elif FISHNET
            return nb.IsServer;
#endif
        }
        public static bool IsClient(this NetworkBehaviour nb)
        {
#if MIRROR && !FISHNET || !MIRROR && !FISHNET
            return nb.isClient;
#elif FISHNET
            return nb.IsClient;
#endif
        }
        public static bool IsLocalPlayer(this NetworkBehaviour nb)
        {
#if MIRROR && !FISHNET || !MIRROR && !FISHNET
            return nb.isLocalPlayer;
#elif FISHNET
            return nb.IsOwner;
#endif
        }

        public static bool HasAuthority(this NetworkBehaviour nb)
        {
#if MIRROR && !FISHNET || !MIRROR && !FISHNET
            return nb.hasAuthority;
#elif FISHNET
            return nb.IsOwner;
#endif
        }
        public static void GiveAuthority(this NetworkIdentity ni, NetworkConnection conn)
        {
#if MIRROR && !FISHNET
            ni.AssignClientAuthority((NetworkConnectionToClient)conn);
#elif FISHNET
            ni.GiveOwnership(conn);
#else
            ni.AssignClientAuthority(conn);
#endif
        }
        public static void RemoveAuthority(this NetworkIdentity ni)
        {
#if MIRROR && !FISHNET || !MIRROR && !FISHNET
            ni.RemoveClientAuthority();
#elif FISHNET
            ni.RemoveOwnership();
#endif
        }

        public static void Spawn(GameObject go, NetworkConnection owner = null, NetworkBehaviour nb = null)
        {
#if MIRROR && !FISHNET || !MIRROR && !FISHNET
            NetworkServer.Spawn(go);
#elif FISHNET
            if (nb == null) InstanceFinder.ServerManager.Spawn(go, owner);
            else nb.Spawn(go, owner);
#endif
        }
    }
}