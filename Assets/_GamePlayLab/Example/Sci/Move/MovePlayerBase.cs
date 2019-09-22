using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPL.Movement
{
    public abstract class MovePlayerBase : MonoBehaviour
    {
        public FollowCamera cam;
        public Transform camTarget;
        public Transform avatar;

        public float moveSpeed = 5f;
        public float rotateSpeed = 5f;

        #region PROPERTY
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

        private Vector3 _lookDirection;
        public Vector3 LookDirection
        {
            set { _lookDirection = Vector3.ClampMagnitude(value, 1f); }
            get { return _lookDirection; }
        }

        public float NowMoveSpeed { get { return Movement.NowMoveSpeed; } }
        #endregion

        // Start is called before the first frame update
        public virtual void Start()
        {
            Movement = GetComponent<MoveMotorBase>();
            Movement.playerBase = this;
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

        public virtual void SwtichMoveType(MoveType moveType)
        {
            _movement.SwitchMoveType(moveType);
        }
    }
}
