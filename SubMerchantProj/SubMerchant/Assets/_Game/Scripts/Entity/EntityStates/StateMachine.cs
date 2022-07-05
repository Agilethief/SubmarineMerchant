using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace CargoGame
{
    public class StateMachine : BaseBehaviour
    {
        BaseEntityState currentState;

        

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

        public void ChangeState(BaseEntityState newState)
        {
            currentState.ExitState();
            currentState = newState;
            currentState.EnterState();
        }

        protected virtual BaseEntityState GetInitialState()
        {
            return null;
        }

        public string GetStateStatus()
        {
            return currentState.stateName;
        }

        // Just a little helper
        //private void OnGUI()
        //{
        //    if(!isLocalPlayer) return;
        //
        //    string content = currentState != null ? currentState.stateName : "(No current state)";
        //    GUILayout.Label($"<color='blue'><size=24>{content}</size></color>");
        //}
    }
}