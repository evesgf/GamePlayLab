using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GPL.Movement
{
    public class JumpRigidbodyMotor_01 : MoveMotorBase
    {
        public Rigidbody _rigidbody;
        public GroundCheck groundCheck;

        [Header("加速度")]
        public float acceleration = 50f;
        [Header("阻力")]
        public float deceleration = 20f;
        [Header("地面摩擦力")]
        public float groundFriction = 8f;

        public Vector3 velocity
        {
            get { return _rigidbody.velocity; }
            set
            {
                _rigidbody.velocity = value;
                Velocity = value;
            }
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

        public override void Jump(float impulse)
        {
            base.Jump(impulse);
            print("Jump!");
            velocity = new Vector3(velocity.x, 0, velocity.z)+ Vector3.up * impulse;
        }

        public override void Move(Vector3 moveDir,float moveSpeed, float deltaTime)
        {
            base.Move(moveDir, moveSpeed, deltaTime);

            //将运动方向映射到地面的平面上
            Vector3 _realMoveDir = Vector3.ProjectOnPlane(moveDir, groundCheck.groundNormal).normalized;

            var desiredAcceleration = _realMoveDir * acceleration * deltaTime*moveDir.magnitude;
            //约束加速度不能超过最大值
            //var speedLimit = moveDir.magnitude > 0f ? Mathf.Min(desiredAcceleration.magnitude, moveSpeed) : moveSpeed;

            if (_realMoveDir == Vector3.zero)
            {
                //应用阻力
                velocity = velocity * Mathf.Clamp01(1f - groundFriction * deltaTime);
                velocity = Vector3.MoveTowards(velocity, _realMoveDir, deceleration * deltaTime);
            }
            else
            {
                //应用加速度
                velocity = Vector3.ClampMagnitude(velocity + desiredAcceleration, moveSpeed);
            }

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

            _rigidbody.MoveRotation(Quaternion.RotateTowards(transform.rotation, rotation, rotateSpeed * deltaTime));
        }

        public override void GroundCheck()
        {
            groundCheck.OnGroundCheck();
        }
    }
}
