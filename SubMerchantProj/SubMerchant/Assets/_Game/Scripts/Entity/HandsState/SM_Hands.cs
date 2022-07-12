using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace CargoGame
{
    public class SM_Hands : StateMachineHands
    {
        //[HideInInspector]
        public HS_Freehands freeHandsState;
       public HS_WaitingToSpawn waitingToSpawnState;
        public HS_Carrying carryingState;
        public HS_Building buildingState;

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
        private CamRig _camRig;
        public CamRig camRig
        {
            get
            {
                if (_camRig == null)
                {
                    _camRig = player.camRig;
                }
                return _camRig;
            }
        }

        public bool handsFree;


        private void Awake()
        {
            freeHandsState = new HS_Freehands(this);
            waitingToSpawnState = new HS_WaitingToSpawn(this);
            carryingState = new HS_Carrying(this);
            buildingState = new HS_Building(this);

           
        }

        override public void Update()
        {
            if(!isLocalPlayer) return;

            base.Update();


        }

        protected override BaseHandsState GetInitialState()
        {
            return waitingToSpawnState;
        }

        public void Spawn()
        {
            ChangeState(freeHandsState);
        }

           public void LookInput()
        {
            camRig.InputUpdate();

            camRig.TurnPlayer(camRig.horizontalRot, player.transform);
            camRig.VerticalLook(camRig.verticalRot);

        }


    }
}