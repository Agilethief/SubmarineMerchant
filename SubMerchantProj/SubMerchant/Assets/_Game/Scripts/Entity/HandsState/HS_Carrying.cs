using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace CargoGame
{
    public class HS_Carrying: BaseHandsState
    {
        public HS_Carrying(SM_Hands stateMachine) : base("Carrying", stateMachine)
        {
            SetHandsSM(stateMachine);
        }

        float throwStr = 0;

        public override void EnterState()
        {
            base.EnterState();
            throwStr = 0;
        }

        public override void UpdateState()
        {
            base.UpdateState();

            sm.LookInput(); // Allow camera movement input.

            if(sm.player.currentCarryObject == null) return;

            if(Input.GetKeyDown(KeyCode.E))
            {
                DropPickup(0);
            }

            // Throw the object?
            if(Input.GetMouseButton(0))
            {
                throwStr += Time.deltaTime;
                if(throwStr>1) throwStr = 1;

            }
            if(Input.GetMouseButtonUp(0))
            {
                DropPickup(throwStr);
            }
        }

        
        // We tell the pickup to drop. We should only actually be able to do that if we have authority over it, which is assigned when we pick it up
        public void DropPickup(float throwStrength)
        {
            if (!sm.hasAuthority)
            {
                Debug.Log("Attempted to drop but have no authority to do so");
                return;
            }
            Debug.Log("Attempted to drop and we have authority to do so!");

            if (sm.player.currentCarryObject == null) return;
            sm.ChangeState(sm.freeHandsState);
            
            // Tell the object to drop
            sm.player.currentCarryObject.CMDDrop(throwStrength * 10); // Tell the object that it is to drop. 
            
            // Remove the object
            sm.player.DroppedObject();

            
        }

    }
}
