﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPL.Movement
{
    public abstract class MovePlayerBase : MonoBehaviour
    {
        public FollowCamera cam;
        public Transform camTarget;
        public Transform avatar;

        public float moveSpeed = 3f;
        public float rotateSpeed = 5f;

        private Vector3 _moveDirection;
        public Vector3 MoveDirection
        {
            set { _moveDirection = Vector3.ClampMagnitude(value, 1f); }
            get { return _moveDirection; }
        }

        public MoveMotorBase _movement;
        public MoveMotorBase Movement
        {
            get
            {
                if (_movement == null) GetComponent<MoveMotorBase>();
                return _movement;
            }
            set { _movement = value; }
        }

        // Start is called before the first frame update
        public virtual void Start()
        {
            Movement = GetComponent<MoveMotorBase>();
        }

        // Update is called once per frame
        public virtual void Update()
        {
            
        }

        public virtual void FixedUpdate()
        {
            Movement.Move(MoveDirection,moveSpeed,Time.fixedDeltaTime);
            Movement.Rotate(MoveDirection, rotateSpeed, Time.fixedDeltaTime);
        }
    }
}
