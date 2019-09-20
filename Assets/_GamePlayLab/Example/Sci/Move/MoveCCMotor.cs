﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPL.Movement
{
    public class MoveCCMotor : MoveMotorBase
    {
        public CharacterController _cc;

        public override void Start()
        {
            base.Start();

            _cc = GetComponent<CharacterController>();
        }

        public override void Update()
        {
            base.Update();
        }

        public override void Move(Vector3 moveDir, float moveSpeed, float deltaTime)
        {
            base.Move(moveDir, moveSpeed, deltaTime);

            _cc.SimpleMove(moveDir * moveSpeed * deltaTime);
        }

        public override void Rotate(Vector3 moveDir, float rotateSpeed, float deltaTime)
        {
            base.Rotate(moveDir, rotateSpeed, deltaTime);

            if (moveDir != Vector3.zero)
            {
                transform.forward = Vector3.Lerp(transform.forward, moveDir, rotateSpeed * deltaTime);
            }
        }
    }
}
