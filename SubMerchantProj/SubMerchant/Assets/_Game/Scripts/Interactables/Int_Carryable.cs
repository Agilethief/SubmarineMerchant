using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace CargoGame
{
    public class Int_Carryable : InteractablePressable, ICarryable
    {

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

            GetComponent<NetworkIdentity>().AssignClientAuthority(conn);

            Pickup(interactingPlayer.carrySocket);
            TargetRPCPickup(conn);
        }

        
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
            rb.useGravity = false;
            rb.isKinematic = true;
            
            pickupTransform = pickingUpObject;
            transform.position = pickingUpObject.position;
            transform.rotation = pickingUpObject.rotation;
            
            interactionComplete = false;
        }
        public void Drop(float throwStr)
        {
            //if(!hasAuthority) return;

            rb.useGravity = true;
            rb.isKinematic = false;
            
             pickupTransform = null;

            interactionComplete = true;
            GetComponent<NetworkIdentity>().RemoveClientAuthority();

            rb.AddForce(fwd * throwStr, ForceMode.Impulse);

        }


    }
}
