using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPL
{
    public class WallDetection : MonoBehaviour
    {
        public LayerMask obstacleLayerMask;
        public LayerMask groundLayerMask;

        public Transform forwardUpCheckRoot;
        public float forwardUpCheckDistance = 1.0f;
        public bool forwardUpIsHit;
        private RaycastHit forwardUpHit;

        public Transform forwardMiddleCheckRoot;
        public float forwardMiddleCheckDistance = 1.0f;
        public bool forwardMiddleIsHit;
        private RaycastHit forwardMiddleHit;

        public Transform forwardDownCheckRoot;
        public float forwardDownCheckDistance = 1.0f;
        public bool forwardDownIsHit;
        private RaycastHit forwardDownHit;

        public Transform forwardDownGroundRoot;
        public float forwardDownGroundDistance = 2.0f;
        public bool forwardDownGroundIsHit;
        private RaycastHit forwardDownGroundHit;

        private Vector3 climbCenter;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void FixedUpdate()
        {
            forwardUpIsHit = Physics.Raycast(forwardUpCheckRoot.position, forwardUpCheckRoot.forward, out forwardUpHit,forwardUpCheckDistance, obstacleLayerMask);

            forwardMiddleIsHit = Physics.Raycast(forwardMiddleCheckRoot.position, forwardMiddleCheckRoot.forward, out forwardMiddleHit, forwardMiddleCheckDistance, obstacleLayerMask);

            forwardDownIsHit = Physics.Raycast(forwardDownCheckRoot.position, forwardDownCheckRoot.forward, out forwardDownHit, forwardDownCheckDistance, obstacleLayerMask);

            forwardDownGroundIsHit = Physics.Raycast(forwardDownGroundRoot.position, -forwardDownGroundRoot.up, out forwardDownGroundHit, forwardDownGroundDistance, groundLayerMask);

            if (!forwardUpIsHit && !forwardMiddleIsHit && forwardDownIsHit)
            {
                climbCenter = forwardMiddleCheckRoot.position + forwardMiddleCheckRoot.forward * forwardMiddleCheckDistance * 0.5f;
            }
            if (!forwardUpIsHit && forwardMiddleIsHit && forwardDownIsHit)
            {
                climbCenter = forwardUpCheckRoot.position + forwardUpCheckRoot.forward * forwardUpCheckDistance * 0.5f;
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (forwardUpIsHit)
            {
                Gizmos.color = new Color(1f, 0f, 0f, 1.0f);
                Gizmos.DrawLine(forwardUpCheckRoot.position, forwardUpHit.point);
            }
            else
            {
                Gizmos.color = new Color(0f, 0f, 1f, 1.0f);
                Gizmos.DrawLine(forwardUpCheckRoot.position, forwardUpCheckRoot.position + forwardUpCheckRoot.forward * forwardUpCheckDistance);
            }

            if (forwardMiddleIsHit)
            {
                Gizmos.color = new Color(1f, 0f, 0f, 1.0f);
                Gizmos.DrawLine(forwardMiddleCheckRoot.position, forwardMiddleHit.point);
            }
            else
            {
                Gizmos.color = new Color(0f, 0f, 1f, 1.0f);
                Gizmos.DrawLine(forwardMiddleCheckRoot.position, forwardMiddleCheckRoot.position + forwardMiddleCheckRoot.forward * forwardMiddleCheckDistance);
            }

            if (forwardDownIsHit)
            {
                Gizmos.color = new Color(1f, 0f, 0f, 1.0f);
                Gizmos.DrawLine(forwardDownCheckRoot.position, forwardDownHit.point);
            }
            else
            {
                Gizmos.color = new Color(0f, 0f, 1f, 1.0f);
                Gizmos.DrawLine(forwardDownCheckRoot.position, forwardDownCheckRoot.position + forwardDownCheckRoot.forward * forwardDownCheckDistance);
            }

            if (forwardDownGroundIsHit)
            {
                Gizmos.color = new Color(1f, 0f, 0f, 1.0f);
                Gizmos.DrawLine(forwardDownGroundRoot.position, forwardDownGroundHit.point);
            }
            else
            {
                Gizmos.color = new Color(0f, 0f, 1f, 1.0f);
                Gizmos.DrawLine(forwardDownGroundRoot.position, forwardDownGroundRoot.position + -forwardDownGroundRoot.up * forwardDownGroundDistance);
            }


            Gizmos.color = new Color(0f, 0f, 1f, 1.0f);
            Gizmos.DrawLine(transform.position,climbCenter);
            Gizmos.DrawLine(climbCenter, forwardDownGroundHit.point);
        }
    }
}