using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace CargoGame
{
    public class SM_Movement : StateMachine
    {
        [HideInInspector]
        public ES_Idle idleState;
        [HideInInspector]
        public ES_Moving moveState;
        [HideInInspector]
        public ES_Interact interactState;
        [HideInInspector]
        public ES_Interact channelState;
        [HideInInspector]
        public ES_Seated seatedState;
        [HideInInspector]
        public ES_Falling fallingState;
        [HideInInspector]
        public ES_Jumping jumpingState;
        [HideInInspector]
        public ES_InMenu inMenuState;
        [HideInInspector]
        public ES_WaitingToSpawn waitingToSpawnState;


        [HideInInspector]
        public float moveSpeed = 4f;


        private SimplePlayer _player;
        public SimplePlayer player
        {
            get
            {
                if (_player == null)
                {
                    _player = transform.GetComponent<SimplePlayer>();
                }
                return _player;
            }
        }


        public float sidewaysInput { get { return Input.GetAxis("Horizontal"); } }
        public float forwardsInput { get { return Input.GetAxis("Vertical"); } }
        public Vector3 vel;
        public float currentGravity, targetGravity;

        public bool previouslyGrounded;
        float _coyoteTimer, _coyoteTimerMax = 0.45f;

        private IMovable _moveable;
        public IMovable movable
        {
            get
            {
                if (_moveable != null) return _moveable;
                else { _moveable = transform.GetComponent<IMovable>(); return _moveable; }
            }
        }

        public InteractableBase currentInteractable;

        private void Awake()
        {
            idleState = new ES_Idle(this);
            moveState = new ES_Moving(this);
            interactState = new ES_Interact(this);
            channelState = new ES_Interact(this);
            seatedState = new ES_Seated(this);
            fallingState = new ES_Falling(this);
            jumpingState = new ES_Jumping(this);
            inMenuState = new ES_InMenu(this);
            waitingToSpawnState = new ES_WaitingToSpawn(this);
        }

        override public void Update()
        {
            if(!isLocalPlayer) return;

            base.Update();

            // Only after the drop timer runs out can we be considered not grounded
            if (_coyoteTimer <= _coyoteTimerMax)
            {
                _coyoteTimer += Time.deltaTime;
            }

        }

        protected override BaseEntityState GetInitialState()
        {
            return waitingToSpawnState;
        }

        public void MovementInput(float gravityInfluence = 0, float speedModifier = 1f)
        {
            vel = new Vector3(sidewaysInput, gravityInfluence, forwardsInput);

            movable.Move(moveSpeed * speedModifier * vel * 3);
        }

        public void EnterMenuState()
        {
            //player.playerUI.OpenInventory();
            
            ChangeState(inMenuState);
            return;

        }


        // This is used when E is pressed. We check there is something to interact with here and move to the interact state as needed
        public void AttemptInteract()
        {
            if(player.currentlookAtInteractable == null) return; // Check - Is there even something to interact with?
            currentInteractable = player.currentlookAtInteractable;

            if(currentInteractable.interactionType == InteractableBase.InteractionType.Channel)
            {
                ChangeState(channelState);
                return;
            }

            InteractInputStart();
            InteractInputEnd(); // Not sure where I'm going to put this for this use case so we just immediately end the interaction?

        }

        public void InteractInputStart()
        {
            //Debug.Log("InteractStartInput");
            if(player.currentlookAtInteractable == null) return;
            //if(!player.currentlookAtInteractable.interactionComplete) return;

            currentInteractable = player.currentlookAtInteractable;
            CmdInteractStart(currentInteractable);
            
        }
        public void InteractInputEnd()
        {
            //Debug.Log("InteractEndInput");
            if(currentInteractable == null) return;

            CmdInteractEnd(currentInteractable);
            currentInteractable = null;
        }

        [Command]
        public void CmdInteractStart(InteractableBase targetInteractable)
        {
            //Debug.Log("Comman interact Start");
            targetInteractable.Interact(player.connectionToClient, player.playerID);
        }
        [Command]
        public void CmdInteractEnd(InteractableBase targetInteractable)
        {
             targetInteractable.InteractEnd(player.connectionToClient);
        }
        [Command]
        public void CmdCloseContainer(InteractableContainer targetInteractable)
        {
            targetInteractable.CloseContainer(player.connectionToClient);
        }


        public void AttemptHotbarUse()
        {
            player.playerUI.hotbarUi.UseSelectedHotbarItem();
        }

        public void Spawn(Vector3 spawnPos)
        {
            pos = spawnPos;
            ChangeState(idleState);

        }


        public bool SMGrounded()
        {
            // If we are believe we are grounded, we need to check if the player still finds the ground
            if (previouslyGrounded)
            {
                // If the player finds the ground, we are STILL grounded. cheeck it all in and return true to being grounded.
                if (player.IsGrounded())
                {
                    previouslyGrounded = true;
                    _coyoteTimer = 0;
                    return true;
                }
                // If the player DOES NOT find the ground, we start checking the timer.
                else
                {
                    // If the timer is overtime, and we have not found the ground, we are NO LONGER grounded, and thus will soon fall.
                    if (_coyoteTimer >= _coyoteTimerMax)
                    {
                        previouslyGrounded = false;
                        return false;
                    }
                    else
                        return true;
                }
            }

            // If we do not believe we are still grounded, we revert to doing a simple player ground check.
            else if (player.IsGrounded())
            {
                previouslyGrounded = true;
                _coyoteTimer = 0;
                return true;
            }

            // Failing everything else, we are not grounded
            return false;
        }

    }
}