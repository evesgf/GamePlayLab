using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GPL.Movement
{
    [RequireComponent(typeof(CapsuleCollider))]
    public class GroundCheck : MonoBehaviour
    {
        public LayerMask groundLayer;

        [Range(0, 89.9f)]
        public float slopeLimit = 60f;

        public float castDistance = 0.5f;

        public float groundCheckExtendRadius = 0.05f;

        public float groundCheckExtendDistance = 0.05f;

        public float groundCheckExtendHeight = 0.05f;

        RaycastHit groundHit;

        public CapsuleCollider _collider;

        public bool isOnGrounded;
        public bool isOnSlope;

        public Vector3 groundPoint;
        public Vector3 groundNormal;
        public float groundAngle;

        // Start is called before the first frame update
        void Start()
        {
            _collider = GetComponent<CapsuleCollider>();
        }

        public void OnGroundCheck()
        {
            var radius = _collider.radius + groundCheckExtendRadius;
            var height = _collider.height * 0.5f - radius - groundCheckExtendHeight;
            var center = _collider.center - Vector3.up * height;
            var orign = transform.position + transform.rotation * center;
            var dir = transform.rotation * Vector3.down;

            _collider.isTrigger = true;

            Debug.DrawLine(orign, orign + dir * height, Color.blue);
            if (Physics.SphereCast(orign, radius, dir, out groundHit, groundCheckExtendDistance + groundCheckExtendHeight, groundLayer))
            {
                groundPoint = groundHit.point;
                groundNormal = groundHit.normal;
                groundAngle = Vector3.Angle(Vector3.up, groundNormal);
                isOnGrounded = true;

                isOnSlope = groundAngle > slopeLimit;
            }
            else
            {
                if (Physics.Raycast(orign, dir, out groundHit, groundCheckExtendDistance + groundCheckExtendHeight + castDistance, groundLayer))
                {
                    groundPoint = groundHit.point;
                    groundNormal = groundHit.normal;
                    groundAngle = Vector3.Angle(Vector3.up, groundNormal);
                    isOnGrounded = true;

                    isOnSlope = groundAngle > slopeLimit;
                }
                else
                {
                    groundPoint = Vector3.zero;
                    groundNormal = Vector3.zero;
                    isOnGrounded = false;
                }
            }

            _collider.isTrigger = false;
        }

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;
            OnDebug();
        }

        public void OnDebug()
        {
            var radius = _collider.radius + groundCheckExtendRadius;
            var height = _collider.height * 0.5f - radius - groundCheckExtendHeight;
            var center = _collider.center - Vector3.up * height;
            var orign = transform.position + transform.rotation * center;
            var dir = transform.rotation * Vector3.down;

            if (isOnGrounded)
            {
                Gizmos.color = Color.green;

                Handles.color = Color.green;
                Handles.DrawSolidDisc(groundPoint, groundNormal, 0.1f);
            }
            else
            {
                Gizmos.color = Color.red;
            }

            Gizmos.DrawWireSphere(groundPoint - dir * radius, radius);
        }
    }

}