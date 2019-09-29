using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GPL.Movement
{
    public class MoveRigidbodyMotor_05 : MoveMotorBase
    {
        public Rigidbody _rigidbody;
        public GroundCheck groundCheck;

        public Vector3 velocity
        {
            get { return _rigidbody.velocity; }
            set { _rigidbody.velocity = value; }
        }

        public bool useGravity = true;
        public float gravity = 9.8f;

        public override void Start()
        {
            base.Start();

            _rigidbody = GetComponent<Rigidbody>();
        }

        public override void Update()
        {
            base.Update();

            NowMoveSpeed = _rigidbody.velocity.magnitude;
            SlopeAngle = groundCheck.groundAngle;
        }

        public override void Move(Vector3 moveDir,float moveSpeed, float deltaTime)
        {
            base.Move(moveDir, moveSpeed, deltaTime);

            //将运动方向映射到地面的平面上
            Vector3 _realMoveDir = Vector3.ProjectOnPlane(moveDir, groundCheck.groundNormal).normalized;

            velocity = _realMoveDir * moveSpeed * moveDir.magnitude;

            if (groundCheck.isOnGrounded && !groundCheck.isOnSlope)
            {
                useGravity = false;
            }
            else
            {
                useGravity = true;
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
            groundCheck.OnGroundCheck();
        }
    }
}
