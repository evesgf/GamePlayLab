using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPL.Movement
{
    public class MoveRigidbodyMotor_02 : MoveMotorBase
    {
        public Rigidbody _rigidbody;

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

            _rigidbody.MovePosition(transform.position+moveDir * moveSpeed * deltaTime);
        }

        public override void Rotate(Quaternion rotation, float rotateSpeed, float deltaTime)
        {
            base.Rotate(rotation, rotateSpeed, deltaTime);

            if (rotation != Quaternion.identity)
            {
                _rigidbody.MoveRotation(Quaternion.Lerp(transform.rotation, rotation, rotateSpeed * deltaTime));
            }
        }
    }
}
