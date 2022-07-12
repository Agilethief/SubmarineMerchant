using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CargoGame
{
    public class BaseHandsState
    {
        public string stateName;
        protected StateMachineHands stateMachine;

        protected SM_Hands sm;


        public BaseHandsState(string stateName, StateMachineHands stateMachine)
        {
            this.stateName = stateName;
            this.stateMachine = stateMachine;
        }

        public virtual void EnterState() { }
        public virtual void UpdateState() { }
        public virtual void ExitState() { }

        public void SetHandsSM(SM_Hands newSM)
        {
            sm = newSM;
        }
    }
}