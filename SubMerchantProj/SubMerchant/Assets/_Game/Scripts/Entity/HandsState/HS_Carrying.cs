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
                CmdDropPickup(0);
            }

            // Throw the object?
            if(Input.GetMouseButton(0))
            {
                throwStr += Time.deltaTime;
                if(throwStr>1) throwStr = 1;

            }
            if(Input.GetMouseButtonUp(0))
            {
                CmdDropPickup(throwStr);
            }
        }

        [Command]
        public void CmdDropPickup(float throwStrength)
        {
            if(sm.player.currentCarryObject == null) return;

            sm.ChangeState(sm.freeHandsState);
            sm.player.currentCarryObject.Drop(throwStrength * 10); // Tell the object that it is to drop.
            sm.player.DroppedObject();
        }

    }
}
