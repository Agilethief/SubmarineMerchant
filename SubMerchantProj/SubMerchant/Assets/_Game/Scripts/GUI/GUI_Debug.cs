using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace CargoGame
{
    public class GUI_Debug : GUIBehaviour
    {
        public SimplePlayer player;
        SM_Movement movementSM;

        public TMP_Text playerState;
        public TMP_Text playerHandsState;
        public TMP_Text playerSpeed;


        public void Update()
        {
            if(player == null) return;
          
            if(!player.isLocalPlayer) return;

            playerState.text = player.movementStateMachine.GetStateStatus();
            playerHandsState.text = player.handsStateMachine.GetStateStatus();
            playerSpeed.text = player.movementStateMachine.moveSpeed.ToString();


        }

    }
}
