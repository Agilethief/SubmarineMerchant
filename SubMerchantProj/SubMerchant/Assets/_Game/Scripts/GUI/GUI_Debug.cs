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


        public void Update()
        {
            if(player == null) return;
          
            if(!player.isLocalPlayer) return;

            playerState.text = player.movementStateMachine.GetStateStatus();


        }

    }
}
