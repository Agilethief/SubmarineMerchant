using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CargoGame
{
    public class ES_Interact : BaseEntityState
    {
       // This is more of a CHANNELING state, rather than interact


        public ES_Interact(SM_Movement stateMachine) : base("Interact", stateMachine)
        {
            SetMovementSM(stateMachine);

        }

        public override void EnterState()
        {
            //Debug.Log("Enter Interact State");
            base.EnterState();

            sm.player.camRig.blockCamInput = true;

            sm.InteractInputStart();
        }

        public override void UpdateState()
        {
            if(!sm.isLocalPlayer) return;
            


            base.UpdateState();


            if(!Input.GetKey(KeyCode.E))
            {
                stateMachine.ChangeState(sm.idleState);
                return;
            }
            
            if(sm.currentInteractable.interactionComplete)
            {
                stateMachine.ChangeState(sm.idleState);
                return;
            }



        }

        public override void ExitState()
        {
            //Debug.Log("Exit Interact State");
            sm.player.camRig.blockCamInput = false;
            sm.InteractInputEnd();

            base.ExitState();


        }


    }
}