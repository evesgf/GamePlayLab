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

        public Transform climbCenterDownGroundRoot;
        public float climbCenterDownGroundGroundDistance = 2.0f;
        public bool climbCenterDownGroundIsHit;
        private RaycastHit climbCenterDownGroundHit;

        public Vector3 climbCenter;
        public Vector3 climbEnd;

        public ClimbOberObsType climbOberObsType;

        private bool _canClimbObstacle;
        private bool _canClimbWall;

        public bool CanClimbObs
        {
            get { return _canClimbObstacle; }
            set { _canClimbObstacle = value; }
        }

        public bool CanClimbWall
        {
            get { return _canClimbWall; }
            set { _canClimbWall = value; }
        }

        public Vector3 ClimbCenterWithHighOffset(float heightOffset)
        {
            return climbCenter+(-climbCenterDownGroundRoot.up * heightOffset);
        }

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

            climbCenterDownGroundIsHit = Physics.Raycast(climbCenterDownGroundRoot.position, -climbCenterDownGroundRoot.up, out climbCenterDownGroundHit, climbCenterDownGroundGroundDistance, groundLayerMask);

            climbCenter = climbCenterDownGroundIsHit ? climbCenterDownGroundHit.point: climbCenterDownGroundRoot.position + (-climbCenterDownGroundRoot.up * climbCenterDownGroundGroundDistance);

            climbEnd = forwardDownGroundIsHit ? forwardDownGroundHit.point : forwardDownGroundRoot.position + (-forwardDownGroundRoot.up * forwardDownGroundDistance);


            if (!forwardUpIsHit && !forwardMiddleIsHit && forwardDownIsHit)
            {
                CanClimbObs = true;
                CanClimbWall = false;
                climbOberObsType = ClimbOberObsType.Low;
            }
            else if (!forwardUpIsHit && forwardMiddleIsHit && forwardDownIsHit)
            {
                CanClimbObs = true;
                CanClimbWall = false;
                climbOberObsType = ClimbOberObsType.High;
            }
            else if (forwardUpIsHit && forwardMiddleIsHit && forwardDownIsHit)
            {
                CanClimbObs = false;
                CanClimbWall = true;
            }
            else
            {
                CanClimbObs = false;
                CanClimbWall = false;
                climbOberObsType = ClimbOberObsType.None;
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

            if (climbCenterDownGroundIsHit)
            {
                Gizmos.color = new Color(1f, 0f, 0f, 1.0f);
                Gizmos.DrawLine(climbCenterDownGroundRoot.position, climbCenterDownGroundHit.point);
            }
            else
            {
                Gizmos.color = new Color(0f, 0f, 1f, 1.0f);
                Gizmos.DrawLine(climbCenterDownGroundRoot.position, climbCenterDownGroundRoot.position + -climbCenterDownGroundRoot.up * climbCenterDownGroundGroundDistance);
            }


            if (CanClimbObs)
            {
                Gizmos.color = new Color(0f, 0f, 1f, 1.0f);
                Gizmos.DrawLine(transform.position, climbCenter);
                Gizmos.DrawLine(climbCenter, forwardDownGroundHit.point);
            }

            Gizmos.color = new Color(0f, 0f, 1f, 1.0f);
            Gizmos.DrawWireSphere(climbCenter, 0.1f);

            Gizmos.color = new Color(0f, 0f, 1f, 1.0f);
            Gizmos.DrawWireSphere(climbEnd, 0.1f);
        }
    }
}