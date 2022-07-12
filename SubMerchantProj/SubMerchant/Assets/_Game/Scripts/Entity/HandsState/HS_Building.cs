using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CargoGame
{
    public class HS_Building : BaseHandsState
    {
        public HS_Building (SM_Hands stateMachine) : base("Building", stateMachine)
        {
            SetHandsSM(stateMachine);
        }

        public override void EnterState()
        {
            base.EnterState();
            
            sm.LookInput(); // Allow camera movement input.
        }
        
    }
}
