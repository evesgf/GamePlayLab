using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GPL.Movement
{
    public class MoveRigidbodyMotor_05 : MoveMotorBase
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
        public float groundCheckRadius = 0.05f;
        public float groundCheckDistance=0.5f;
        public bool isOnGrounded;
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

        public override void Move(Vector3 moveDir,float moveSpeed, float deltaTime)
        {
            base.Move(moveDir, moveSpeed, deltaTime);

            //将运动方向映射到地面的平面上
            Vector3 _realMoveDir = Vector3.ProjectOnPlane(moveDir, groundNormal).normalized;

            velocity = _realMoveDir * moveSpeed;

            if (!isOnGrounded)
            {
                useGravity = true;
            }
            else
            {
                if (SlopeAngle <= slopeLimit)
                {
                    useGravity = false;
                }
                else
                {
                    useGravity = true;
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
            var orign = transform.position+(Vector3.down * (_collider.height * 0.5f - _collider.radius- groundCheckRadius-0.01f));
            var radius = _collider.radius + groundCheckRadius;
            var dis = groundCheckDistance + 0.01f;

            _collider.isTrigger = true;

            if (Physics.SphereCast(orign, radius, Vector3.down, out groundHit, dis, groundLayer))
            {
                groundPoint = groundHit.point;
                groundNormal = groundHit.normal;
                groundDistance = groundHit.distance;
                isOnGrounded = true;
            }
            else
            {
                if (Physics.Raycast(orign, Vector3.down, out groundHit, dis + 0.5f, groundLayer))
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

            var orign = transform.position + (Vector3.down * (_collider.height * 0.5f - _collider.radius));
            var radius = _collider.radius + groundCheckRadius;
            var dis = groundCheckDistance;

            if (isOnGrounded)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(orign+Vector3.down*groundDistance, radius);

                Handles.color= Color.green;
                Handles.DrawSolidDisc(groundPoint, groundNormal, 0.1f);
            }
            else
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(orign+Vector3.down* dis, radius);
            }
        }
    }
}
