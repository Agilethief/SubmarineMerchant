using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CargoGame
{
    public class Int_Channel_Wheel : InteractableChannel
    {
        public Transform wheelTransform;
        public float maxRotateAmount = 720f;
        public float minRotateAmount = 0;

        public bool rotX, rotY, rotZ;

        Vector3 targetRot;
        Quaternion targetQuat;


        public DrivenObjectBase targetDrivenObject;

        public override void ChannelUpdate()
        {
            base.ChannelUpdate();

            if (rotY) { targetRot.y = Mathf.Lerp(minRotateAmount, maxRotateAmount, channelProgressNormalised); }
            if (rotX) { targetRot.x = Mathf.Lerp(minRotateAmount, maxRotateAmount, channelProgressNormalised); }
            if (rotZ) { targetRot.z = Mathf.Lerp(minRotateAmount, maxRotateAmount, channelProgressNormalised); }

            targetQuat = Quaternion.Euler(targetRot);
            wheelTransform.localRotation = targetQuat;


            targetDrivenObject?.DriveObject(channelProgressNormalised);
        }
    }
}
