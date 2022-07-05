using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CargoGame
{
    public class ES_InMenu : BaseEntityState
    {
       // This is for when a menu is open and the player must look at it
       // The use of this does mean that the player will freeze in place. TODO - Make that better


        public ES_InMenu(SM_Movement stateMachine) : base("In Menu", stateMachine)
        {
            SetMovementSM(stateMachine);

        }

        public override void EnterState()
        {
            //Debug.Log("Enter Interact State");
            base.EnterState();

            sm.player.camRig.blockCamInput = true;
            sm.player.playerUI.RevealMouse(true);
        }

        public override void UpdateState()
        {
            if(!sm.isLocalPlayer) return;
            
            base.UpdateState();


            if(Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Escape)|| Input.GetKeyDown(KeyCode.I))
            {
                
                stateMachine.ChangeState(sm.idleState);
                return;
            }

        }

        public override void ExitState()
        {
            sm.player.camRig.blockCamInput = false;
            sm.player.playerUI.RevealMouse(false);
            sm.player.playerUI.CloseAllPanels();

            if(sm.player.currentlookAtInteractable != null)
            {
                if(sm.player.currentlookAtInteractable.GetComponent<InteractableContainer>() != null)
                {
                    sm.CmdCloseContainer(sm.player.currentlookAtInteractable.GetComponent<InteractableContainer>());
                }
            }

            base.ExitState();


        }


    }
}