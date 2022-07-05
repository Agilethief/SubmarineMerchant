using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CargoGame
{
    public class BaseEntityState
    {
        public string stateName;
        protected StateMachine stateMachine;

        protected SM_Movement sm;


        public BaseEntityState(string stateName, StateMachine stateMachine)
        {
            this.stateName = stateName;
            this.stateMachine = stateMachine;
        }

        public virtual void EnterState() { }
        public virtual void UpdateState() { }
        public virtual void ExitState() { }

        public void SetMovementSM(SM_Movement newSM)
        {
            sm = newSM;
        }
    }
}