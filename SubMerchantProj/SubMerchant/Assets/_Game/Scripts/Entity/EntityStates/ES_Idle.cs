using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CargoGame
{
    public class ES_Idle : BaseEntityState
    {
        public ES_Idle(SM_Movement stateMachine) : base("Idle", stateMachine)
        {
            SetMovementSM(stateMachine);
        }


        public override void EnterState()
        {
            base.EnterState();

            //sm.player.camRig.targetFOV =  sm.player.camRig.baseFOV;

            sm.previouslyGrounded = true;
        }

        public override void UpdateState()
        {
            if (!sm.isLocalPlayer) return;

            base.UpdateState();

            // Movement
            if (Mathf.Abs(sm.sidewaysInput) > Mathf.Epsilon || Mathf.Abs(sm.forwardsInput) > Mathf.Epsilon)
            {
                stateMachine.ChangeState(sm.moveState);
                return;
            }

            if (Input.GetButton("Fire1"))
            {
                sm.AttemptHotbarUse();
                return;
            }

            // Interacting
            if (Input.GetKeyDown(KeyCode.E))
            {
                sm.AttemptInteract();
                return;
            }

             if(Input.GetKeyDown(KeyCode.I))
            {
               // Open inventory here
               sm.player.playerUI.ShowInventory();
                sm.EnterMenuState();
                    return;
            }

            // Jumping
            if (Input.GetKeyDown(KeyCode.Space))
            {
                stateMachine.ChangeState(sm.jumpingState);
                return;
            }

            // Check if falling
            if (!sm.SMGrounded())
            {
                stateMachine.ChangeState(sm.fallingState);
                return;
            }
        }

    }
}