using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CargoGame
{
    public class ES_Jumping : BaseEntityState
    {
        public ES_Jumping(SM_Movement stateMachine) : base("Jumping", stateMachine)
        {
            SetMovementSM(stateMachine);
        }

        float lift = 2f;
        float currentLift;
        float jumpDuration = 0.3f, jumpTimer;
        float normalisedTime;

        public override void EnterState()
        {
            base.EnterState();

            jumpTimer = 0;
            currentLift = 0;
            sm.currentGravity = 0;
        }

        public override void UpdateState()
        {
            if (!sm.isLocalPlayer) return;

            base.UpdateState();

            normalisedTime = Mathf.InverseLerp(0, jumpDuration, jumpTimer); // Normalise the time of the jump to make lerps easier.
            currentLift = Mathf.Lerp(lift, 0, normalisedTime); // Current lift starts at max power, but weakens as time goes on
            //sm.currentGravity = Mathf.Lerp(sm.currentGravity, currentLift, Time.deltaTime * 5f); // Lerp our gravity influence towards the lift very quickly
            sm.currentGravity = currentLift;

            sm.MovementInput(sm.currentGravity, 1.1f);


            jumpTimer += Time.deltaTime;
            if (jumpTimer >= jumpDuration)
            {
                stateMachine.ChangeState(sm.fallingState);
                return;
            }
            else if (normalisedTime > 0.25f)
            {
                //if (sm.SMGrounded())
                //{ 
                //    stateMachine.ChangeState(sm.idleState);
                //    return;
                //}
                if (CheckHead())
                { 
                    stateMachine.ChangeState(sm.fallingState);
                    return;
                }
            }

            if(Input.GetButton("Fire1"))
            {
                sm.AttemptHotbarUse();
                return;
            }
        }

        public bool CheckHead()
        {
            //Debug.DrawLine(sm.player.pos + Vector3.up * sm.player.cc.height, sm.player.pos + Vector3.up * sm.player.cc.height + Vector3.up * 0.1f);
            //Debug.LogError("GroundedRay");
            //Debug.Break();

            // If it doesn't lets do a bigger beter check.
            return Physics.CheckSphere(sm.player.pos + Vector3.up * sm.player.cc.height, 0.1f, sm.player.groundLayerMask);


        }
    }
}