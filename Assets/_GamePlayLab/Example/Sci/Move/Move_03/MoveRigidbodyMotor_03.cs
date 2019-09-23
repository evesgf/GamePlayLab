using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPL.Movement
{
    public class MoveRigidbodyMotor_03 : MoveMotorBase
    {
        public Rigidbody _rigidbody;
        public CapsuleCollider _collider;

        public Vector3 velocity
        {
            get { return _rigidbody.velocity; }
            set { _rigidbody.velocity = value; }
        }

        public LayerMask groundLayer;
        public float groundCheckDistance=0.5f;
        public bool isOnGrounded;
        public Vector3 groundPoint;
        public Vector3 groundNormal;
        RaycastHit groundHit;

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
            Debug.DrawRay(groundPoint, _realMoveDir, Color.green);

            _rigidbody.MovePosition(Vector3.Lerp(transform.position, transform.position + _realMoveDir, moveSpeed * deltaTime));
        }

        public override void Rotate(Quaternion rotation, float rotateSpeed, float deltaTime)
        {
            base.Rotate(rotation, rotateSpeed, deltaTime);

            if (rotation != Quaternion.identity)
            {
                _rigidbody.MoveRotation(Quaternion.Lerp(transform.rotation, rotation, rotateSpeed * deltaTime));
            }
        }

        public override void GroundCheck()
        {
            var orign = transform.position+(Vector3.down * (_collider.height * 0.5f - _collider.radius));
            var dis = _collider.radius + groundCheckDistance;
            if (Physics.Raycast(orign, Vector3.down, out groundHit, dis, groundLayer))
            {
                groundPoint = groundHit.point;
                groundNormal = groundHit.normal;
                isOnGrounded = true;
                Debug.DrawLine(transform.position, groundPoint, Color.black);
            }
            else
            {
                groundPoint = Vector3.zero;
                groundNormal = Vector3.zero;
                isOnGrounded = false;
                Debug.DrawRay(transform.position, - transform.up*groundCheckDistance, Color.black);
            }
        }
    }
}
