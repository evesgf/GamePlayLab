using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GPL.Movement
{
    public class MoveRigidbodyMotor_06 : MoveMotorBase
    {
        public Rigidbody _rigidbody;
        public CapsuleCollider _collider;

        [Range(0,89.9f)]
        public float slopeLimit = 45f;

        public Vector3 velocity
        {
            get { return _rigidbody.velocity; }
            set { _rigidbody.velocity = value; }
        }

        public LayerMask groundLayer;
        public float groundCheckExtendRadius = 0.05f;
        public float groundCheckExtendDistance=0.05f;
        public float groundCheckExtendHeight = 0.05f;
        public bool isOnGrounded;
        public bool isSlope;
        public Vector3 groundPoint;
        public Vector3 groundNormal;
        public float groundDistance;
        RaycastHit groundHit;

        public bool useGravity = true;
        public float gravity = 9.8f;

        public override void Start()
        {
            base.Start();

            _rigidbody = GetComponent<Rigidbody>();
            _collider = GetComponent<CapsuleCollider>();
        }

        public override void Update()
        {
            base.Update();

            NowMoveSpeed = _rigidbody.velocity.magnitude;
        }

        public override void Jump(float impulse)
        {
            base.Jump(impulse);

            var jumpImpulse = Vector3.up * impulse;
            _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, 0, _rigidbody.velocity.z) + jumpImpulse;
        }

        public override void Move(Vector3 moveDir,float moveSpeed, float deltaTime)
        {
            base.Move(moveDir, moveSpeed, deltaTime);

            //in air
            if (!isOnGrounded)
            {
                useGravity = true;

                //将运动方向映射到地面的平面上
                Vector3 _realMoveDir = Vector3.ProjectOnPlane(moveDir, groundNormal).normalized;

                velocity = _realMoveDir * moveSpeed* moveDir.magnitude;
            }
            else
            {
                useGravity = false;
                //in plane
                if (SlopeAngle <= slopeLimit)
                {
                    isSlope = false;

                    //将运动方向映射到地面的平面上
                    Vector3 _realMoveDir = Vector3.ProjectOnPlane(moveDir, groundNormal).normalized;

                    velocity = _realMoveDir * moveSpeed * moveDir.magnitude;

                }
                //in slope
                else
                {
                    isSlope = true;

                    //将运动方向映射到地面的平面上
                    Vector3 _realMoveDir = Vector3.ProjectOnPlane(moveDir, groundNormal).normalized;

                    velocity = _realMoveDir * moveSpeed * moveDir.magnitude;

                    velocity += Vector3.down * gravity;
                }
            }

            if (useGravity)
            {
                velocity += Vector3.down * gravity;
            }
        }

        public override void Rotate(Quaternion rotation, float rotateSpeed, float deltaTime)
        {
            base.Rotate(rotation, rotateSpeed, deltaTime);

            _rigidbody.MoveRotation(Quaternion.Lerp(transform.rotation, rotation, rotateSpeed * deltaTime));
        }

        public override void GroundCheck()
        {
            var radius = _collider.radius + groundCheckExtendRadius;
            var height = _collider.height*0.5f-radius- groundCheckExtendHeight;
            var center = _collider.center - Vector3.up * height;
            var orign = transform.position + transform.rotation * center;
            var dir = transform.rotation * Vector3.down;

            _collider.isTrigger = true;

            Debug.DrawLine(orign, orign+dir* height, Color.blue);
            if (Physics.SphereCast(orign, radius, dir, out groundHit, groundCheckExtendDistance+ groundCheckExtendHeight, groundLayer))
            {
                groundPoint = groundHit.point;
                groundNormal = groundHit.normal;
                groundDistance = groundHit.distance;
                isOnGrounded = true;
            }
            else
            {
                if (Physics.Raycast(orign, dir, out groundHit, groundCheckExtendDistance + groundCheckExtendHeight+0.5f, groundLayer))
                {
                    groundPoint = groundHit.point;
                    groundNormal = groundHit.normal;
                    groundDistance = groundHit.distance;
                    isOnGrounded = true;
                }
                else
                {
                    groundPoint = Vector3.zero;
                    groundNormal = Vector3.zero;
                    groundDistance = 0;
                    isOnGrounded = false;
                }
            }

            _collider.isTrigger = false;

            SlopeAngle = Vector3.Angle(Vector3.up, groundNormal);
        }

        private void OnDrawGizmos()
        {
            var radius = _collider.radius + groundCheckExtendRadius;
            var height = _collider.height * 0.5f - radius- groundCheckExtendHeight;
            var center = _collider.center - Vector3.up * height;
            var orign = transform.position + transform.rotation * center;
            var dir = transform.rotation * Vector3.down;

            if (isOnGrounded)
            {
                Gizmos.color = Color.green;

                Handles.color= Color.green;
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
