using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPL.Movement
{
    public class MoveRigidbodyMotor : MoveMotorBase
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
        }

        public override void Move(Vector3 moveDir,float moveSpeed, float deltaTime)
        {
            base.Move(moveDir, moveSpeed, deltaTime);

            _rigidbody.MovePosition(transform.position + moveDir * moveSpeed * deltaTime);
        }

        public override void Rotate(Vector3 moveDir, float rotateSpeed, float deltaTime)
        {
            base.Rotate(moveDir, rotateSpeed, deltaTime);

            if (moveDir != Vector3.zero)
            {
                _rigidbody.MoveRotation(Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(moveDir), rotateSpeed * deltaTime));
            }
        }
    }
}
