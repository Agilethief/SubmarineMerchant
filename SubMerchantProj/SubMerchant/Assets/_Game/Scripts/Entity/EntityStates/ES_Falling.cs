using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CargoGame
{
    public class ES_Falling : BaseEntityState
    {
        float gravity = -1.97f;
        public ES_Falling(SM_Movement stateMachine) : base("Falling", stateMachine)
        {
            SetMovementSM(stateMachine);
        }

        public override void EnterState()
        {
            base.EnterState();
            sm.previouslyGrounded = false;

        }

        public override void UpdateState()
        {
            if (!sm.isLocalPlayer) return;

            base.UpdateState();

            sm.currentGravity = Mathf.Lerp(sm.currentGravity, gravity, Time.deltaTime * 2f); // graceful fall

            sm.MovementInput(sm.currentGravity, 1f);

            if (sm.SMGrounded())
                stateMachine.ChangeState(sm.idleState);

            if (Input.GetButton("Fire1"))
            {
                sm.AttemptHotbarUse();
                return;
            }
        }
    }
}