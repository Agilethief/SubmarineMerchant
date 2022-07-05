using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Cleverous.VaultInventory;
using Cleverous.VaultSystem;

namespace CargoGame
{
    public class InteractableContainer : InteractableBase, IUseInventory
    {

        public Transform MyTransform => transform;
        public Inventory Inventory
        {
            get => m_inventory;
            set => m_inventory = value;
        }
        [SerializeField]
        private Inventory m_inventory;

        [AssetDropdown(typeof(LootTable))]
        public LootTable Loot;

        protected bool SvrLocked;
        private GameObject m_chestInventoryUi;

        public bool GenerateRandomItems;

        private void Reset()
        {
            interactionType = InteractionType.Container;
        }

        public override void OnStartServer()
        {
            base.OnStartServer();

            if (!m_inventory.IsInitialized) Inventory.Initialize(this, Inventory.Configuration);
            if (Loot == null) return;

            if (GenerateRandomItems)
            {
                // get a random number of items to add to the inventory
                int count = Random.Range(0, Loot.Items.Length);

                // add something from the loot table that many times
                for (int i = 0; i < count; i++)
                {
                    RootItemStack loot = Loot.GetLoot();
                    m_inventory.DoAdd(loot);
                }
            }
        }

        public override void OnStartClient()
        {

            if (!m_inventory.IsInitialized) Inventory.Initialize(this, Inventory.Configuration);
        }





        public override void Interact(NetworkConnectionToClient conn, int _interactingPlayerID)
        {
            base.Interact(conn, _interactingPlayerID);

            if (isServer && !SvrLocked)
            {
                SvrOpen(conn);
            }

            //TargetRpcOpenContainer(conn, container.containerStruct);
        }

       public virtual void CloseContainer(NetworkConnectionToClient conn)
        {
            Debug.Log(entityName + " closed");

            if (isServer && SvrLocked)
            {
                SvrClose(conn);
            }
        }


        [ClientRpc]
        private void RpcOpenCallback()
        {
            if (!hasAuthority) return;
            // in this global callback all players will open the box
            // but only the one with authority (the one touching it was assigned authority by server) will get a UI for it.
            if (hasAuthority) m_chestInventoryUi = VaultInventory.SpawnInventoryUi(Inventory, entityName);

            SimplePlayer localPlayer = NetworkClient.localPlayer.GetComponent<SimplePlayer>();
            localPlayer.movementStateMachine.EnterMenuState();
            localPlayer.playerUI.ShowInventory();

        }
        [ClientRpc]
        private void RpcCloseCallback()
        {
            // in this global callback all players will close the lid.
            UiContextMenu.Instance.HideContextMenu();
            Cleanup();

        }

        [Server]
        private void SvrOpen(NetworkConnectionToClient conn)
        {
            // on the server make sure the box is not in use by someone else, then callback to players.
            SvrLocked = true;
            GetComponent<NetworkIdentity>().AssignClientAuthority(conn);
            RpcOpenCallback();
        }

        [Server]
        private void SvrClose(NetworkConnectionToClient conn)
        {
            // on the server make sure the touching player has authority to close the box.
             
            if (conn != this.connectionToClient) return;
            

            // if they're the one interacting, then they are in control and can exit, closing the box.
            SvrLocked = false;
            GetComponent<NetworkIdentity>().RemoveClientAuthority();
            RpcCloseCallback();
            UiContextMenu.Instance.HideContextMenu();
        }

        protected virtual void Cleanup()
        {
            if (m_chestInventoryUi != null) Destroy(m_chestInventoryUi);
        }
    }
}
