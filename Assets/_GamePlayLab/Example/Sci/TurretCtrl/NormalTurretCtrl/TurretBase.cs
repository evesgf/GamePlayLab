using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPL
{
    public class TurretBase : MonoBehaviour
    {
        public Transform realTargetIcon;
        public bool debugShow = false;

        public Transform swivel;
        public float HeadingTrackingSpeed = 1f;
        public Vector2 HeadingLimit;
        public Transform barrel;
        public float ElevationTrackingSpeed = 1f;
        public Vector2 ElevationLimit;

        internal Vector3 target;

        private bool fullAccess = false;

        // Start is called before the first frame update
        void Start()
        {
            if (HeadingLimit.y - HeadingLimit.x >= 359.9f)
                fullAccess = true;
        }

        // Update is called once per frame
        void Update()
        {
            Rotate();
        }

        public float GetAngleToTarget()
        {
            return Vector3.Angle(barrel.forward, target - barrel.position);
        }

        private void Rotate()
        {
            //finding position for turning just for X axis (down-up)
            Vector3 targetX = target - barrel.position;
            Quaternion targetRotationX = Quaternion.LookRotation(targetX, swivel.up);

            barrel.rotation = Quaternion.Slerp(barrel.rotation, targetRotationX, HeadingTrackingSpeed * Time.deltaTime);
            barrel.localEulerAngles = new Vector3(barrel.localEulerAngles.x, 0f, 0f);

            //checking for turning up too much
            if (barrel.transform.localEulerAngles.x >= 180f && barrel.transform.localEulerAngles.x < (360f - ElevationLimit.y))
            {
                barrel.transform.localEulerAngles = new Vector3(360f - ElevationLimit.y, 0f, 0f);
            }

            //down
            else if (barrel.transform.localEulerAngles.x < 180f && barrel.transform.localEulerAngles.x > -ElevationLimit.x)
            {
                barrel.transform.localEulerAngles = new Vector3(-ElevationLimit.x, 0f, 0f);
            }

            //finding position for turning just for Y axis
            Vector3 targetY = target;
            targetY.y = barrel.position.y;

            Quaternion targetRotationY = Quaternion.LookRotation(targetY - swivel.position, swivel.transform.up);

            swivel.transform.rotation = Quaternion.Slerp(swivel.transform.rotation, targetRotationY, ElevationTrackingSpeed * Time.deltaTime);
            swivel.transform.localEulerAngles = new Vector3(0f, swivel.transform.localEulerAngles.y, 0f);

            //if (!fullAccess)
            //{
            //    //checking for turning left
            //    if (swivel.transform.localEulerAngles.y >= 180f && swivel.transform.localEulerAngles.y < (360f - HeadingLimit.y))
            //    {
            //        swivel.transform.localEulerAngles = new Vector3(0f, 360f - HeadingLimit.y, 0f);
            //    }

            //    //right
            //    else if (swivel.transform.localEulerAngles.y < 180f && swivel.transform.localEulerAngles.y > -HeadingLimit.x)
            //    {
            //        swivel.transform.localEulerAngles = new Vector3(0f, -HeadingLimit.x, 0f);
            //    }
            //}

            var realPos = Camera.main.WorldToScreenPoint(barrel.position + barrel.forward * Vector3.Distance(barrel.position, target));
            realTargetIcon.position = new Vector3(realPos.x, realPos.y, 0);
        }

        private void OnDrawGizmos()
        {
            if (!debugShow) return;
            Gizmos.color = Color.red;

            Debug.DrawLine(barrel.position, barrel.position + barrel.forward * Vector3.Distance(barrel.position, target), Color.red);
        }
    }
}
