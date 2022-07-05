// (c) Copyright Cleverous 2020. All rights reserved.

using UnityEngine;
using Cleverous.NetworkImposter;

#if MIRROR && !FISHNET
using Mirror;
#elif FISHNET
using FishNet.Object;
using FishNet.Connection;
using NetworkIdentity = FishNet.Object.NetworkObject;
using ClientRpc = FishNet.Object.ObserversRpcAttribute;
#endif


namespace Cleverous.VaultInventory.Example
{
    public class VaultExampleChestAdvanced : VaultExampleChestSimple, IUseInventory
    {
        public Transform MyTransform => transform;
        public Inventory Inventory
        {
            get => m_inventory;
            set => m_inventory = value;
        }

        [Header("Advanced Chest Configuration")]
        [SerializeField]
        private Inventory m_inventory;

        protected bool SvrLocked;
        private GameObject m_chestInventoryUi;

        public override void OnStartClient()
        {
#if FISHNET
            base.OnStartClient();
#endif
            if (!m_inventory.IsInitialized) Inventory.Initialize(this, Inventory.Configuration);
        }
        public override void OnStartServer()
        {
#if FISHNET
            base.OnStartServer();
#endif
            if (!m_inventory.IsInitialized) Inventory.Initialize(this, Inventory.Configuration);
            if (Loot == null) return;

            // get a random number of items to add to the inventory
            int count = Random.Range(0, Loot.Items.Length);

            // add something from the loot table that many times
            for (int i = 0; i < count; i++)
            {
                RootItemStack loot = Loot.GetLoot();
                m_inventory.DoAdd(loot);
            }
        }

        public override void OnTriggerEnter(Collider col)
        {
            if (this.IsServer() && !SvrLocked)
            {
                SvrOpen(col);
            }
        }
        public override void OnTriggerExit(Collider col)
        {
            if (this.IsServer() && SvrLocked)
            {
                SvrClose(col);
            }
        }

        [Server]
        private void SvrOpen(Collider col)
        {
            // on the server make sure the box is not in use by someone else, then callback to players.
            SvrLocked = true;
            NetworkIdentity interactor = col.GetComponent<NetworkIdentity>();
            GetComponent<NetworkIdentity>().GiveAuthority(interactor.Owner());
            RpcOpenCallback();
        }
        [Server]
        private void SvrClose(Collider col)
        {
            // on the server make sure the touching player has authority to close the box.
            NetworkIdentity interactor = col.GetComponent<NetworkIdentity>();
            if (interactor.Owner() != this.Owner()) return;

            // if they're the one interacting, then they are in control and can exit, closing the box.
            SvrLocked = false;
            GetComponent<NetworkIdentity>().RemoveAuthority();
            RpcCloseCallback();
            UiContextMenu.Instance.HideContextMenu();
        }

        [ClientRpc]
        private void RpcOpenCallback()
        {
            // in this global callback all players will open the box
            AudioSource.clip = SoundOpen;
            AudioSource.Play();
            Lid.transform.localRotation = Quaternion.Euler(90, 0, 0);

            // but only the one with authority (the one touching it was assigned authority by server) will get a UI for it.
            if (this.HasAuthority()) m_chestInventoryUi = VaultInventory.SpawnInventoryUi(Inventory);
        }
        [ClientRpc]
        private void RpcCloseCallback()
        {
            // in this global callback all players will close the lid.
            AudioSource.clip = SoundClose;
            AudioSource.Play();
            Lid.transform.localRotation = Quaternion.Euler(0, 0, 0);
            UiContextMenu.Instance.HideContextMenu();
            Cleanup();
        }

        protected virtual void Cleanup()
        {
            if (m_chestInventoryUi != null) Destroy(m_chestInventoryUi);
        }

        public void OnDestroy()
        {
            Cleanup();
        }
    }
}