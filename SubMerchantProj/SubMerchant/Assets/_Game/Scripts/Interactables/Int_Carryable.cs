using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace CargoGame
{
    public class Int_Carryable : InteractablePressable, ICarryable
    {
        
        public Transform pickupTransform;
        
        [SyncVar]
        public GameObject pickupHolderGO;
        
        public PickupHolder pickupHolder;

        public override void Interact(NetworkConnectionToClient conn, int _interactingPlayerID)
        {
            if (!interactionComplete)
            {
                Debug.Log("Interaction is not complete, cancel interaction");
                return;
            }

            base.Interact(conn, _interactingPlayerID);
            Debug.Log(interactingPlayer.playerName + ": Interacting with pickup");

            

            Pickup(interactingPlayer.transform);
            TargetRPCPickup(conn);
        }

        
        // Let the interacting and thus picking up player do any First Person things they need to do when picking up
        [TargetRpc]
        private void TargetRPCPickup(NetworkConnection conn)
        {
            gameManager.localPlayer.PickedUpObject(this);
            
        }


        // All clients run this, this handles cleaning up or setting things as needed
        [ClientRpc]
        void RPCSetPickupState(bool isCarried)
        {
            if(isCarried)
            {
                rb.isKinematic = true;
            }
            else
            {
                rb.isKinematic = false;
                pickupHolderGO = null;
                pickupTransform = null;
                pickupHolder = null;
            }
        }

        void Update()
        {
            if (!hasAuthority) return;
            if(isServerOnly) return; // If we have authority but this is the server then we return. ??

            if (pickupHolderGO != null)
            {
                if(pickupHolder == null) pickupHolder = pickupHolderGO.GetComponent<PickupHolder>();
                if (pickupHolder.holdingTransform != null)
                {
                    pickupTransform = pickupHolder.holdingTransform;
                }
                else
                {
                    // This will likely cause problems! Need to resolve
                    pickupTransform = pickupHolderGO.transform;
                }

                transform.position = Vector3.Lerp(transform.position, pickupTransform.position, Time.deltaTime * 30); // Clean up some jitter?
                transform.rotation = Quaternion.Lerp(transform.rotation, pickupTransform.rotation, Time.deltaTime * 10); // Clean up some jitter?
            }
        }

        public void Pickup(Transform pickingUpObject)
        {
            //if(!hasAuthority) return;

            Debug.Log(interactingPlayer.playerName + ": Interacting with pickup");
            //Debug.Log("Object now picked up");
            
            RPCSetPickupState(true);

            pickupHolderGO = pickingUpObject.gameObject; 
            pickupHolder = pickingUpObject.GetComponent<PickupHolder>(); // This will also be synced by the update loop doing a check
           

            netID.AssignClientAuthority(interactingConnectionToClient);

            interactionComplete = false;
        }

        [Command]
        public void CMDDrop(float throwStr)
        {
            //if(!hasAuthority) return;

            RPCSetPickupState(false);

            interactionComplete = true;
            
            rb.AddForce(fwd * throwStr, ForceMode.Impulse);
            
            netID.RemoveClientAuthority();
        }


    }
}
