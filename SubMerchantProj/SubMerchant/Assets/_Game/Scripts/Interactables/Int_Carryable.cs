using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace CargoGame
{
    public class Int_Carryable : InteractablePressable, ICarryable
    {
        [SyncVar]
        public Transform pickupTransform;

        public override void Interact(NetworkConnectionToClient conn, int _interactingPlayerID)
        {
            if (!interactionComplete)
            {
                Debug.Log("Interaction is not complete, cancel interaction");
                return;
            }

            base.Interact(conn, _interactingPlayerID);
            Debug.Log(interactingPlayer.playerName + ": Interacting with pickup");

            

            Pickup(interactingPlayer.carrySocket);
            TargetRPCPickup(conn);
        }

        
        // Let the interacting and thus picking up player do any First Person things they need to do when picking up
        [TargetRpc]
        private void TargetRPCPickup(NetworkConnection conn)
        {
            gameManager.localPlayer.PickedUpObject(this);
            
        }

        void Update()
        {
            if(!hasAuthority) return;
            
            if (pickupTransform != null)
            {
                transform.position = pickupTransform.position;
                transform.rotation = pickupTransform.rotation;
            }
        }

        public void Pickup(Transform pickingUpObject)
        {
            //if(!hasAuthority) return;

            Debug.Log(interactingPlayer.playerName + ": Interacting with pickup");
            //Debug.Log("Object now picked up");
            rb.isKinematic = true;
            
            pickupTransform = pickingUpObject;
            
            netID.AssignClientAuthority(interactingConnectionToClient);

            interactionComplete = false;
        }

        [Command]
        public void CMDDrop(float throwStr)
        {
            //if(!hasAuthority) return;

            rb.isKinematic = false;
            
            pickupTransform = null;

            interactionComplete = true;
            netID.RemoveClientAuthority();

            rb.AddForce(fwd * throwStr, ForceMode.Impulse);

        }


    }
}
