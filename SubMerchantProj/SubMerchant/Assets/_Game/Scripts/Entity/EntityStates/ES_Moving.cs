using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CargoGame
{
    public class ES_Moving : BaseEntityState
    {
        Vector3 vel;

        private float footstepDistance = 1.7f;
        private Vector3 lastStepPosition;

        public ES_Moving(SM_Movement stateMachine) : base("Moving", stateMachine)
        {
            SetMovementSM(stateMachine);
        }

        public override void EnterState()
        {
            base.EnterState();

            lastStepPosition = sm.player.transform.position;

            //sm.player.camRig.StartCamBob();
            //sm.player.camRig.targetFOV =  sm.player.camRig.baseFOV + sm.player.camRig.movingFOV;

            sm.previouslyGrounded = true;
        }

        public override void UpdateState()
        {
            if (!sm.isLocalPlayer) return;

            base.UpdateState();

            if (Mathf.Abs(sm.sidewaysInput) < Mathf.Epsilon && Mathf.Abs(sm.forwardsInput) < Mathf.Epsilon)
            {
                stateMachine.ChangeState(sm.idleState);
                return;
            }
            
            sm.MovementInput();

            CheckFootstepDistance();

            if(Input.GetButton("Fire1"))
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

            if (Input.GetKeyDown(KeyCode.I))
            {
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

        public override void ExitState()
        {
            base.ExitState();

            //sm.player.camRig.EndCamBob();
        }


        void CheckFootstepDistance()
        {
            if (Vector3.Distance(lastStepPosition, sm.player.transform.position) >= footstepDistance)
            {
                lastStepPosition = sm.player.transform.position;
                //DieselBehaviour.audioManager.PlaySFX(DieselBehaviour.audioManager.sfx_footStepsConcrete, sm.player.transform, 0.15f, true);
                //Debug.Log("Footstep firing!");
                return;
            }

        }

    }
}