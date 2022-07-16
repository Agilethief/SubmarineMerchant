using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace CargoGame
{
    public class Int_Press_HingeDoor : InteractablePressable
    {
        
        public Transform DoorHingeTransform;
        public float openRotateAmount = 720f;
        public float closedRotateAmount = 0;

        [SyncVar]
        public bool doorOpen;

        public float openDuration = 3f;

        Vector3 targetRot;
        Quaternion targetQuat;

        IEnumerator currentRoutine;

        public override void Interact(NetworkConnectionToClient conn, int _interactingPlayerID)
        {
            base.Interact(conn, _interactingPlayerID);
            Debug.Log("Interacting with Door");

            if(currentRoutine != null)
                StopCoroutine(currentRoutine);

            currentRoutine = DoorRoutine(!doorOpen);
            StartCoroutine(currentRoutine);

        }


        IEnumerator DoorRoutine(bool Opening)
        {
            doorOpen = Opening;

            float currentTime = 0;
            float normTime = 0;

            float newRotAmount = closedRotateAmount;
            if(Opening)
                newRotAmount = openRotateAmount;

            float oldRotAmount = openRotateAmount;
            if(Opening)
                oldRotAmount = closedRotateAmount;

            while(currentTime < openDuration)
            {
                normTime = Mathf.InverseLerp(0, openDuration, currentTime);

                DoorRotate(normTime, oldRotAmount, newRotAmount);

                currentTime+= Time.deltaTime;
                yield return null;
            }

            canInteract = true;

            yield return null;
        }

        void DoorRotate(float normTime, float oldRot, float newRot)
        {

            targetRot.y = Mathf.Lerp(oldRot, newRot, normTime);
            //targetRot.x = Mathf.Lerp(oldRot, newRot, normTime);
            //targetRot.z = Mathf.Lerp(oldRot, newRot, normTime);

            targetQuat = Quaternion.Euler(targetRot);
            DoorHingeTransform.localRotation = targetQuat;
        }


    }
}
