using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Mirror;

namespace CargoGame
{
    public class CamRig : BaseBehaviour
    {
        [SerializeField]
        public SimplePlayer simplePlayer;

        [SerializeField]
        private PostFXVolume dizzyWaterPostVolume, deadPostVolume;
        bool dizzy;
        float dizzyX, dizzyY, dizzyYRot;

        public Camera ownCam { get { return playerCam.ownCam; } }
        public Camera cam { get { return playerCam.ownCam; } }

        public PlayerCamera playerCam;
        [SerializeField]
        public Transform pivot;


        [SerializeField]
        private float turnSpeed = 50f, lookSpeed = 50f;
        [SerializeField]
        private float maxUpLook = 90, maxDownLook = -90;


        public bool blockCamInput;

        [HideInInspector]
        public float horizontalRot, verticalRot;

        IEnumerator fovLerpingRoutine;
        public float baseFOV = 80, dizzyFOV = 90, deadFOV = 50;
        public Transform carrySocket;

        public override void OnStartClient()
        {
            base.OnStartClient();

            // Take control of the player cam;
            if (playerCam == null)
            {
                playerCam = FindObjectOfType<PlayerCamera>();
                playerCam.SetRig(this);
            }
        }

       
        private void Update()
        {
            if(!hasAuthority) return;

            if(simplePlayer != null)
            { 
                pos = simplePlayer.pos;
                rot = simplePlayer.rot;
            }

            if(blockCamInput) {  return; }

            // We used to handle input here, now we do it in the hands statemachine

            if(dizzy)
            {
                dizzyX = Mathf.Lerp(dizzyX, Random.Range(-1.0f,1.0f) * 2f, Time.deltaTime * 0.2f);
                dizzyY = Mathf.Lerp(dizzyX, Random.Range(-1.0f,1.0f), Time.deltaTime * 0.2f);

                TurnPlayer(dizzyX, simplePlayer.transform);
                // TODO - This moves the pivot beyond the max values (combined with the other clamping and screws everything up!!
                dizzyYRot = Mathf.Min(maxUpLook, Mathf.Max(maxDownLook, dizzyYRot + dizzyY * lookSpeed * Time.deltaTime));
                
                pivot.transform.localRotation = Quaternion.Euler(pivot.transform.localRotation.eulerAngles + new Vector3(dizzyYRot,0,0));
            }

        }

        public void InputUpdate()
        {
             if(!hasAuthority) return;

            horizontalRot = Input.GetAxis("Mouse X") * turnSpeed * Time.deltaTime;

            verticalRot = Mathf.Min(maxUpLook, Mathf.Max(maxDownLook, verticalRot + Input.GetAxis("Mouse Y") * lookSpeed * Time.deltaTime));
        }

        // Horizontal rotation
        public void TurnPlayer(float yRot, Transform playerTransform)
        {
            playerTransform.Rotate(0, yRot, 0);

        }

        // Vertical rotation
        public void VerticalLook(float xRot)
        {
            pivot.transform.localRotation = Quaternion.Euler(-xRot, 0, 0);
        }

        public Transform GetCamTransform()
        {
            if(ownCam != null)
                return ownCam.transform;
            else
                return null;
        }

        public void FindPlayerCam()
        {
            if (playerCam == null)
            {
                playerCam = FindObjectOfType<PlayerCamera>();
                playerCam.SetRig(this);
            }
        }

        public void SetDizzyWater(bool SetOn)
        {
            if (SetOn)
            { 
                dizzyWaterPostVolume.FadeUpVolume(8f);
                LerpFOV(dizzyFOV, 8f);
                dizzy = true;
            }
            else
            { 
                dizzyWaterPostVolume.ClearVolume(1.5f, false);
                LerpFOV(baseFOV, 3f);
                dizzy = false;
            }
        }
        public void SetDeadVolume(bool SetOn)
        {
            if (SetOn)
            { 
                deadPostVolume.FadeUpVolume(1f);
                LerpFOV(deadFOV, 4f);
            }
            else
            { 
                deadPostVolume.ClearVolume(1f, false);
                LerpFOV(baseFOV, 1.5f);
            }
        }

        public void LerpFOV(float targetFOV, float fadeDuration)
        {
            if(fovLerpingRoutine != null)
                StopCoroutine(fovLerpingRoutine);

            fovLerpingRoutine = LerpFOVRoutine(targetFOV, fadeDuration);
            StartCoroutine(fovLerpingRoutine);
        }

        IEnumerator LerpFOVRoutine(float targetFOV, float fadeDuration)
        {
            float timer = 0;
            float normalisedTime = 0;

            while(timer <= fadeDuration)
            {
                timer+= Time.deltaTime;

                normalisedTime = Mathf.InverseLerp(0, 1, timer);

                cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, normalisedTime);

                yield return null;
            }

             cam.fieldOfView = targetFOV;

            yield return null;
        }

    }
}
