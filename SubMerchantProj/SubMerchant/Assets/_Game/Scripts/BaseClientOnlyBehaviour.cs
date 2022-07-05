using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScrapTower
{
    public class BaseClientOnlyBehaviour : MonoBehaviour
    {
        public bool debugThisObject;

        public Rigidbody rb { get { return transform.GetComponent<Rigidbody>(); } }
        public CharacterController cc { get { return transform.GetComponent<CharacterController>(); } }
        public Vector3 pos
        {
            get { return transform.position; }
            set { transform.position = value; }
        }
        public Vector3 posFlat
        {
            get { return Flatten(transform.position); }
        }
        public Vector3 localPos
        {
            get { return transform.localPosition; }
            set { transform.localPosition = value; }
        }
        public Vector3 scale
        {
            get { return transform.localScale; }
            set { transform.localScale = value; }
        }
        public Quaternion rot
        {
            get { return transform.rotation; }
            set { transform.rotation = value; }
        }
        public Vector3 rotEuler
        {
            get { return transform.rotation.eulerAngles; }
            set
            {
                Quaternion convertedQuat = Quaternion.Euler(value);
                transform.rotation = convertedQuat;
            }
        }

        public Vector3 fwd
        {
            get { return transform.forward; }
            set { transform.forward = value; }
        }

        private Camera _cam;
        public Camera cam
        {
            get
            {
                if (_cam != null) return _cam;
                else
                {
                    if(Camera.main != null) _cam = Camera.main;
                    else _cam = FindObjectOfType<Camera>();
                    return _cam;
                }
            }
        }

        public static Vector3 Flatten(Vector3 incomingVec)
        {
            return new Vector3(incomingVec.x, 0, incomingVec.z);
        }
        public static Vector3 FlattenX(Vector3 incomingVec)
        {
            return new Vector3(0, incomingVec.y, incomingVec.z);
        }
        public static Vector3 FlattenZ(Vector3 incomingVec)
        {
            return new Vector3(incomingVec.x, incomingVec.y, 0);
        }

        

        public void DebugLog(string msg)
        {
            if (debugThisObject)
                Debug.Log(msg);
        }
        public void DebugLine(Vector3 startPos, Vector3 endPos, Color colour)
        {
            if (debugThisObject)
            {
                Debug.DrawLine(startPos, endPos, colour);
            }

        }

        public static bool FastApproximately(float a, float b, float threshold)
        {
            if (threshold >= 0f)
            {
                //Debug.Log("Using threshold instead of approx: " + Mathf.Abs( Mathf.Abs(a) - Mathf.Abs(b) ));
                return Mathf.Abs(Mathf.Abs(a) - Mathf.Abs(b)) <= threshold;
            }
            else
            {
                //Debug.Log("Using approx instead of threshold");
                return Mathf.Approximately(a, b);
            }
        }

        public static float EaseOutBounce(float t)
        {
            const float n1 = 7.5625f;
            const float d1 = 2.75f;

            if (t < 1 / d1)
                return n1 * t * t;
            else if (t < 2 / d1)
                return n1 * (t -= 1.5f / d1) * t + 0.75f;
            else if (t < 2.5f / d1)
                return n1 * (t -= 2.25f / d1) * t + 0.9375f;
            else
                return n1 * (t -= 2.625f / d1) * t + 0.984375f;

        }

        public static float EaseInOutBack(float t)
        {
            const float c1 = 1.70158f;
            const float c2 = c1 * 1.525f;
            float t2 = t - 1f;
            return t < 0.5
                ? t * t * 2 * ((c2 + 1) * t * 2 - c2)
                : t2 * t2 * 2 * ((c2 + 1) * t2 * 2 + c2) + 1;
        }

        // Currently only does to two decimal places, need to rejig to work with variable number
        public static float RoundNumber(float incomingNumber)
        {
            incomingNumber = Mathf.Round(incomingNumber * 100f) / 100f;

            return incomingNumber;
        }

        public static void DebugMsg(string msg, Color msgCol)
        {
            string msgColString = ColorUtility.ToHtmlStringRGBA(msgCol);

            Debug.Log("<color=#" + msgColString + ">" + msg + "</color>");
        }

        public static void LookAtOnY(Vector3 targetPosition, Transform lookingObject)
        {
            Vector3 targetFlattened = Flatten(targetPosition);
            Vector3 startFlattened = Flatten(lookingObject.position);
            Vector3 dir = targetFlattened - startFlattened;
            dir = dir.normalized;
            lookingObject.forward = dir;
        }
        public static void LookAtOnX(Vector3 targetPosition, Transform lookingObject)
        {
            Vector3 targetFlattened = FlattenX(targetPosition);
            Vector3 startFlattened = FlattenX(lookingObject.position);
            Vector3 dir = targetFlattened - startFlattened;
            dir = dir.normalized;
            lookingObject.forward = dir;
        }

        public static Vector3 GetGroundPosition(Vector3 castPosition)
        {
            if (Physics.Raycast(castPosition, Vector3.down, out var castToGroundHit))
            {
                return castToGroundHit.point;
            }

            return castPosition;
        }

        public static void DebugSphere(Vector3 center, float radius, bool pause = false)
        {
            Debug.DrawLine(center, center + Vector3.forward * radius, Color.red);
            Debug.DrawLine(center, center + Vector3.back * radius, Color.red);
            Debug.DrawLine(center, center + Vector3.up * radius, Color.red);
            Debug.DrawLine(center, center + Vector3.down * radius, Color.red);
            Debug.DrawLine(center, center + Vector3.left * radius, Color.red);
            Debug.DrawLine(center, center + Vector3.right * radius, Color.red);

            if (pause) Debug.Break();

        }

        /// <summary>
        /// Returns the direction to a specified world position
        /// </summary>
        /// <param name="worldPos"></param>
        /// <returns></returns>
        public Vector3 DirectionTo(Vector3 worldPos)
        {
            Vector3 dir = worldPos - pos;
            dir = dir.normalized;
            return dir;
        }
    }
}
