using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace CargoGame
{
    public class Int_Press_DrinkWater : InteractablePressable
    {
        [SerializeField]
        private int waterAmount = 200;

        public override void Interact(NetworkConnectionToClient conn, int _interactingPlayerID)
        {
            base.Interact(conn, _interactingPlayerID);
            Debug.Log("Interacting with water");
            
          
            TargetRPCDrinkWater(conn);
        }

        [TargetRpc]
        private void TargetRPCDrinkWater(NetworkConnection conn)
        {
            //if (!hasAuthority) return;
            
            gameManager.localPlayer.DrinkWater(waterAmount);

            
        }

    }
}
