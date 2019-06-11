using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GPL
{
    public class AIShipCtrl : MonoBehaviour
    {
        public Transform target;

        public float stopDistance = 10f;
        public float stopDegrees = 5f;

        internal Vector3 movement;
        internal ShipMove shipMove;

        private float targetDistance;
        private float angle;
        private Vector3 targetPanelPos;
        private bool targetIsRight;

        // Start is called before the first frame update
        void Start()
        {
            shipMove = GetComponent<ShipMove>();
        }

        void Update()
        {
            if (target == null) return;
            targetDistance = Vector3.Distance(transform.position, target.position);
            if (targetDistance <= stopDistance)
            {
                movement = Vector3.zero;
                return;
            }

            targetPanelPos = new Vector3(target.position.x, transform.position.y, target.position.z);
            Vector3 targetDir = (targetPanelPos - transform.position).normalized;

            targetIsRight = Vector3.Cross(transform.forward, targetDir).y > 0;
            angle = Vector3.Angle(transform.forward, targetDir);

            movement.x = angle > stopDegrees ? (targetIsRight ? 1 : -1) : 0;

            movement.z = targetDistance > stopDistance ? 1 : 0;
        }

        private void FixedUpdate()
        {
            if (target == null) return;

            shipMove.OnMove(movement);
        }
    }
}
