using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPL.Movement
{
    public class MoveRigidbodyMotor_01 : MoveMotorBase
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
                Vector3 smoothLookDir = Vector3.Slerp(transform.forward, moveDir, 1 - Mathf.Exp(-rotateSpeed * deltaTime)).normalized;

                _rigidbody.MoveRotation(Quaternion.LookRotation(smoothLookDir, transform.up));

                //_rigidbody.MoveRotation(Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(moveDir,Vector3.up), rotateSpeed * deltaTime));
            }
        }
    }
}
