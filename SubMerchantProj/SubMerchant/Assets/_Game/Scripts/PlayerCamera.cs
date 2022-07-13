using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CargoGame
{
    public class PlayerCamera : BaseClientOnlyBehaviour
    {
        [SerializeField]
        public Camera ownCam;

        public CamRig camRig;

        public void SetRig(CamRig newCamRig)
        {
            camRig = newCamRig;
            transform.parent = camRig.pivot;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
    }
}
