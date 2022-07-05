using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CargoGame
{
    public class DO_PositionLerp : DrivenObjectBase
    {
        public Vector3 basePosition, drivenPosition;

        public override void DriveObject(float driveValue)
        {
            base.DriveObject(driveValue);


            transform.localPosition = Vector3.Lerp(basePosition, drivenPosition, driveValue);

        }
    }
}
