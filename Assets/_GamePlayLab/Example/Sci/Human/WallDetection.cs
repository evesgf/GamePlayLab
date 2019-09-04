using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPL
{
    public class WallDetection : MonoBehaviour
    {
        public LayerMask layerMask;

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

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void FixedUpdate()
        {
            forwardUpIsHit = Physics.Raycast(forwardUpCheckRoot.position, forwardUpCheckRoot.forward, out forwardUpHit,forwardUpCheckDistance, layerMask);

            forwardMiddleIsHit = Physics.Raycast(forwardMiddleCheckRoot.position, forwardMiddleCheckRoot.forward, out forwardMiddleHit, forwardMiddleCheckDistance, layerMask);

            forwardDownIsHit = Physics.Raycast(forwardDownCheckRoot.position, forwardDownCheckRoot.forward, out forwardDownHit, forwardDownCheckDistance, layerMask);
        }

        private void OnDrawGizmosSelected()
        {
            if (forwardUpIsHit)
            {
                Gizmos.color = new Color(1f, 0f, 0f, 1.0f);
            }
            else
            {
                Gizmos.color = new Color(0f, 0f, 1f, 1.0f);
            }
            Gizmos.DrawLine(forwardUpCheckRoot.position, forwardUpCheckRoot.position + forwardUpCheckRoot.forward* forwardUpCheckDistance);

            if (forwardMiddleIsHit)
            {
                Gizmos.color = new Color(1f, 0f, 0f, 1.0f);
            }
            else
            {
                Gizmos.color = new Color(0f, 0f, 1f, 1.0f);
            }
            Gizmos.DrawLine(forwardMiddleCheckRoot.position, forwardMiddleCheckRoot.position + forwardMiddleCheckRoot.forward * forwardMiddleCheckDistance);

            if (forwardDownIsHit)
            {
                Gizmos.color = new Color(1f, 0f, 0f, 1.0f);
            }
            else
            {
                Gizmos.color = new Color(0f, 0f, 1f, 1.0f);
            }
            Gizmos.DrawLine(forwardDownCheckRoot.position, forwardDownCheckRoot.position + forwardDownCheckRoot.forward * forwardDownCheckDistance);
        }
    }
}