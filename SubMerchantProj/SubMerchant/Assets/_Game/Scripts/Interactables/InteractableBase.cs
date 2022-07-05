using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace CargoGame
{
    public class InteractableBase : Entity
    {
        public enum InteractionType {Channel, Pickup, Container, Pressable }
        public InteractionType interactionType;

        public bool interactionComplete;

        public Sprite crosshairSprite;

        protected NetworkConnection interactingPlayerConnection;
        protected NetworkIdentity interactingPlayerNetID;
        protected SimplePlayer interactingPlayer;
        protected int interactingPlayerID;

        // TODO: Handle the GM better
        private GameManager _GM;
        public GameManager gameManager
        {
            get
            {
                if (_GM == null) _GM = FindObjectOfType<GameManager>();

                return _GM;
            }
        }


        public virtual void Interact(NetworkConnectionToClient conn, int _interactingPlayerID)
        {
            if(!interactionComplete) return;

            if(debugThisObject) Debug.Log(entityName + " Interacted with");
            interactionComplete = false;

            interactingPlayerConnection = conn;

            interactingPlayerNetID = conn.identity;
            //interactingPlayer = interactingPlayerNetID.GetComponent<SimplePlayer>();
            interactingPlayerID = _interactingPlayerID;

            foreach(SimplePlayer player in gameManager.playerList)
            {
                if(player.playerID == _interactingPlayerID)
                {
                    interactingPlayer = player;
                }
            }
        }

        public virtual void InteractEnd(NetworkConnectionToClient conn)
        {
            

            if(debugThisObject) Debug.Log(entityName + " finished being interacted with");
        }


        public override void OnStartServer()
        {
            base.OnStartServer();
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
        }
    }
}
