using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace CargoGame
{
    public class InteractablePressable : InteractableBase
    {
        private void Reset()
        {
            interactionType = InteractionType.Pressable;
        }
       public override void Interact(NetworkConnectionToClient conn, int _interactingPlayerID)
        {
            base.Interact(conn, _interactingPlayerID);

            
        }
    }
}
