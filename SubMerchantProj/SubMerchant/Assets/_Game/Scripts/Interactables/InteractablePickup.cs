using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Cleverous.VaultSystem;
using Cleverous.VaultInventory;

namespace CargoGame
{
    public class InteractablePickup : InteractableBase
    {

        [AssetDropdown(typeof(RootItem))]
        public RootItem pickupItem;
        public RootItemStack Data;

        [SyncVar] public int stackSize;

        [SerializeField]
        protected GameObject artContainer;

        protected bool IsBusy;

        private void Reset()
        {
            interactionType = InteractionType.Pickup;
        }

        public override void Interact(NetworkConnectionToClient conn, int _interactingPlayerID)
        {
            base.Interact(conn, _interactingPlayerID);

            AttemptPickup(conn);
            
        }

       public virtual void AttemptPickup(NetworkConnectionToClient conn)
        {
            Debug.Log(entityName + " attempting to be picked up");

            if (!isServer) return;

            IUseInventory inventoryToEnter = GetPlayerFromConnection(conn);
            if (inventoryToEnter != null) TryPickup(inventoryToEnter);

        }

        public override void OnStartClient()
        {
            Data = new RootItemStack(pickupItem, stackSize);
           
        }


        /// <summary>
        /// This is called from VaultInventory when spawning the item on the Server. It will setup the syncvars.
        /// </summary>
        public virtual void SvrInitialize(RootItem sourceItem, int stackSize)
        {
            //if (!NetworkPipeline.StaticIsServer()) return;
            //
            //StackSize = stackSize;
            //ItemDbKey = sourceItem.GetDbKey();
        }

        /// <summary>
        /// Try to pick up the item. only on the server.
        /// </summary>
        /// <param name="requestor">The Inventory interface trying to claim the item.</param>
        public virtual void TryPickup(IUseInventory requestor)
        {
            // If the Inventory can't fit the content, it'll reflect back the amount.
            // We have to keep this object and update the stack size here in that case.
            if (IsBusy) return;
            IsBusy = true;

            int remainder = VaultInventory.TryGiveItem(requestor.Inventory, Data);
            if (remainder <= 0) Destroy(gameObject);
            else Data.StackSize = remainder;

            IsBusy = false;
        }

        public virtual void PickupSuccessful()
        {
            Debug.Log(entityName + " picked up");
        }

       public virtual void PickupFailed()
        {
            Debug.Log(entityName + " failed to be picked up");
        }
    }
}
