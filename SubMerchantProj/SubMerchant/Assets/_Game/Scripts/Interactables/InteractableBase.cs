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

        [SyncVar]
        public bool canInteract;

        public Sprite crosshairSprite;

        protected NetworkConnection interactingPlayerConnection;
        protected NetworkConnectionToClient interactingConnectionToClient;
        protected NetworkIdentity interactingPlayerNetID;
        protected SimplePlayer interactingPlayer;
        protected int interactingPlayerID;

         private NetworkIdentity _NetID;
        public NetworkIdentity netID
        {  get {
                if (_NetID == null) _NetID = GetComponent<NetworkIdentity>();
                return _NetID;
            }
        }


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


        private void Start()
        {
            canInteract = true; // This just makes sure the object can be interacted with
        }

        public virtual void Interact(NetworkConnectionToClient conn, int _interactingPlayerID)
        {
            if(!canInteract) {
                Debug.Log("Interaction is not complete, cancel interaction");
                return;
            }

            if(debugThisObject) Debug.Log(entityName + " Interacted with");
            canInteract = false;

            interactingPlayerConnection = conn;
            interactingConnectionToClient = conn;
            interactingPlayerNetID = conn.identity;
            //interactingPlayer = interactingPlayerNetID.GetComponent<SimplePlayer>();
            interactingPlayerID = _interactingPlayerID;

            //Debug.Log("Player list count" + gameManager.playerList);
            foreach(SimplePlayer player in gameManager.playerList)
            {
                if(player.playerID == _interactingPlayerID)
                {
                    interactingPlayer = player;
                    //Debug.Log("Found and set the interacting player");
                    break;
                }
            }
            if(interactingPlayer == null) Debug.Log("Could not find the interacting player");
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
