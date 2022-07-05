using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CargoGame
{
    public class FaceTowardsCamera : BaseClientOnlyBehaviour
    {

        Vector3 dirToCamera;
        public bool flattenOnY, invertDirection;



        // Update is called once per frame
        void Update()
        {
            if (flattenOnY)
            {
                dirToCamera = Flatten(cam.transform.position) - Flatten(pos);
                dirToCamera = dirToCamera.normalized;
            }
            else
            {
                dirToCamera = cam.transform.position - pos;
                dirToCamera = dirToCamera.normalized;
            }

            if(invertDirection)
                dirToCamera = dirToCamera * -1;

            if(dirToCamera == Vector3.zero) // Guard to stop annoying "view rotation is zero" warning messages
                return;

            transform.forward = dirToCamera;

        }
    }
}
