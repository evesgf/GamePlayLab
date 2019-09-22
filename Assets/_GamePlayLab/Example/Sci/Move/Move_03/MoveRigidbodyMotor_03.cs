using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPL.Movement
{
    public class MoveRigidbodyMotor_03 : MoveMotorBase
    {
        public Rigidbody _rigidbody;

        public Vector3 velocity
        {
            get { return _rigidbody.velocity; }
            set { _rigidbody.velocity = value; }
        }

        public LayerMask garoundLayer;
        public float groundCheckDistance=0.5f;
        public bool isOnGrounded;
        public Vector3 groundPoint;
        public Vector3 groundNormal;
        RaycastHit groundHit;

        public override void Start()
        {
            base.Start();

            _rigidbody = GetComponent<Rigidbody>();
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
            Debug.DrawRay(transform.position, _realMoveDir, Color.green);

            _rigidbody.MovePosition(transform.position+ _realMoveDir * moveSpeed * deltaTime);
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
            if (Physics.Raycast(transform.position, -transform.up, out groundHit, groundCheckDistance))
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
