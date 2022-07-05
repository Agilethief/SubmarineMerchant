using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CargoGame
{
    public class ES_WaitingToSpawn : BaseEntityState
    {
        public ES_WaitingToSpawn(SM_Movement stateMachine) : base("WaitingToSpawn", stateMachine)
        {
            SetMovementSM(stateMachine);
        }

        public override void EnterState()
        {
            sm.transform.position = Vector3.down * 1000f;

            base.EnterState();

        }

        public override void UpdateState()
        {
            if (!sm.isLocalPlayer) return;

            // DEBUG - just for quick tested when needed
            if(Input.GetKeyDown(KeyCode.Return))
                GameManager.instance.RespawnLocalPlayer();

            base.UpdateState();

        }

        public override void ExitState()
        {
            base.ExitState();

        }


    }
}
