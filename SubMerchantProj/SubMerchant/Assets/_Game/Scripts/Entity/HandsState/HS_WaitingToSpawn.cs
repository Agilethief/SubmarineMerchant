using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CargoGame
{
    public class HS_WaitingToSpawn: BaseHandsState
    {
        public HS_WaitingToSpawn(SM_Hands stateMachine) : base("WaitingToSpawn", stateMachine)
        {
            SetHandsSM(stateMachine);
        }

        public override void EnterState()
        {
            base.EnterState();
            

        }
        
    }
}
