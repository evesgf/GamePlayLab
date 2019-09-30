using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GPL.Movement
{
    public class GroundCheck_07 : GroundCheck
    {
        RaycastHit groundHit;

        public override void OnGroundCheck()
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

        public void CheckLedgeAndSteps()
        {

        }
    }
}