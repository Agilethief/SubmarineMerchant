using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CargoGame
{
    public class HS_Freehands : BaseHandsState
    {
        public HS_Freehands(SM_Hands stateMachine) : base("Free hands", stateMachine)
        {
            SetHandsSM(stateMachine);
        }

        public override void EnterState()
        {
            base.EnterState();
            sm.handsFree = true;
        }

        public override void UpdateState()
        {
            base.UpdateState();

            sm.LookInput(); // Allow camera movement input.
        }

        public override void ExitState()
        {
            base.ExitState();

            sm.handsFree = false;

        }

    }
}
