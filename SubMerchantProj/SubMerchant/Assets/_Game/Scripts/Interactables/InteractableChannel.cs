using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace CargoGame
{
    public class InteractableChannel : InteractableBase
    {
        public bool beingChanneled;
        public bool resetChannelOnEnd;
        public float channelProgress;
        public float channelDuration = 3f;
        public bool alternateDirection;
        [SyncVar]
        public bool goingForwards = true;

        public float channelProgressNormalised { get { return Mathf.InverseLerp(0, channelDuration, channelProgress); } }

        private void Reset()
        {
            interactionType = InteractionType.Channel;
        }
       public override void Interact(NetworkConnectionToClient conn, int _interactingPlayerID)
        {
            base.Interact(conn, _interactingPlayerID);

            ChannelStart();
            
        }

        private void Update()
        {
            if(!isServer) return;

            if(!beingChanneled) return;

            ChannelUpdate();
        }

        
        // Call when the user starts interacting with this
        public virtual void ChannelStart()
        {
            //Debug.Log(entityName + " Channel started");
            beingChanneled = true;

            if(alternateDirection) goingForwards = !goingForwards;  // Flip the direction
            else goingForwards = true; // If not alternating, make sure we are always going forward
        }
        
        
        // Call when interacted with every frame
        public virtual void ChannelUpdate()
        {
            //Debug.Log(entityName + " Channel tick");

            

            if(goingForwards)
            { 
                channelProgress += Time.deltaTime;
                if (channelProgress >= channelDuration)
                    ChannelCompleted();
            }
            else
            { 
                channelProgress -= Time.deltaTime;
                if (channelProgress <= 0)
                    ChannelCompleted();
            }
            
        }
        
        
        // Call when the interaction ends for any reason
        public virtual void ChannelEnd()
        {
            //Debug.Log(entityName + " Channel ended");
            beingChanneled = false;
            canInteract = true;

            if(resetChannelOnEnd) channelProgress = 0;
        }
        
        
        // Call when the interaction finshes
        public virtual void ChannelCompleted()
        {
            //Debug.Log(entityName + " Channel complete");

            ChannelEnd();
        }

        
        public override void InteractEnd(NetworkConnectionToClient conn)
        {
            base.InteractEnd(conn);

            ChannelEnd();
        }
    }
}
