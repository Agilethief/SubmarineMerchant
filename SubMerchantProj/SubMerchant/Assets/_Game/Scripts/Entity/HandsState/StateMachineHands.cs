using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CargoGame
{
    public class StateMachineHands : BaseBehaviour
    {
       BaseHandsState currentState;
       
        


        // Start is called before the first frame update
        public override void OnStartLocalPlayer ()
        {
            currentState = GetInitialState();
            if (currentState != null)
            {
                currentState.EnterState();
            }
        }

        // Update is called once per frame
        virtual public void Update()
        {
            if (currentState != null)
                currentState.UpdateState();
        }

        public void ChangeState(BaseHandsState newState)
        {
            currentState.ExitState();
            currentState = newState;
            currentState.EnterState();
        }

        protected virtual BaseHandsState GetInitialState()
        {
            return null;
        }

        public string GetStateStatus()
        {
            return currentState.stateName;
        }
    }
}
