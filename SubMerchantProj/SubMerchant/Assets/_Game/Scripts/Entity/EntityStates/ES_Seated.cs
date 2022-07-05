using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CargoGame
{
    public class ES_Seated : BaseEntityState
    {
        //public Usable currentUsable;

        public ES_Seated(SM_Movement stateMachine) : base("Seated", stateMachine)
        {
            SetMovementSM(stateMachine);
        }

        public override void EnterState()
        {
            base.EnterState();

            //sm.player.camRig.GetCamState.ChangeState(sm.player.camRig.GetCamState.seatedState);
            //currentUsable = sm.player.lookAtUsable;
        }

        public override void ExitState()
        {
            base.ExitState();


            //sm.player.camRig.GetCamState.ChangeState(sm.player.camRig.GetCamState.lookState);

        }

        public override void UpdateState()
        {
            if(!sm.isLocalPlayer) return;

            base.UpdateState();

            if (Input.GetKeyDown(KeyCode.E))
            {

                stateMachine.ChangeState(sm.idleState);
                //currentUsable.UseEnd();
            }

            //sm.player.pos = currentUsable.seatedPosition.position;
        }
    }
}