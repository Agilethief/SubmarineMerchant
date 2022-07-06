using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace CargoGame
{
    public class GameManager : BaseBehaviour
    {
        public static GameManager instance;

        public SimplePlayer localPlayer;    // Set by the local player on the player start
        public List<SimplePlayer> playerList = new List<SimplePlayer>();

        
        private void Awake()
        {
            instance = this;
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            //Debug.Log("Game Manager Server Start Fired");

            StartCoroutine(WaterTickingRoutine());
        }

        public override void OnStartClient()
        {
            base.OnStartClient();

            //Debug.Log("Game Manager Client Start Fired");
        }

        IEnumerator WaterTickingRoutine()
        {
            while(true)
            {
                yield return new WaitForSeconds(1);
                Srv_SendWaterTick();
                yield return null; 
            }
        }


        [Server]
        public void Srv_SendWaterTick()
        {
            foreach(SimplePlayer player in playerList)
            {
                if(player == null) return;

                player.WaterTick();
            }
        }

        public void RespawnLocalPlayer()
        {
            if(localPlayer == null) {
                Debug.LogWarning("Attempted to respawn the local player, but had no player to respawn"); return;
            }

            localPlayer.movementStateMachine.Spawn(NetworkManager.startPositions[0].position);
            

        }

        public void SetupLocalPlayer()
        {
              if(localPlayer == null) return;

              localPlayer.InitialSetup();

        }

    }
}
