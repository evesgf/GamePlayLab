using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPL.Movement
{
    public enum MoveType
    {
        Forward=0,
        Camera=1
    }

    public abstract class MoveMotorBase : MonoBehaviour
    {
        internal MovePlayerBase playerBase;

        public float NowMoveSpeed { get; set; }

        public float SlopeAngle { get; set; }

        public Vector3 Velocity { get; set; }

        // Start is called before the first frame update
        public virtual void Start() { }

        public virtual void Update() { }

        public virtual void GroundCheck() { }

        public virtual void Move(Vector3 moveDir, float moveSpeed,float deltaTime) { }

        public virtual void Rotate(Vector3 moveDir, float rotateSpeed, float deltaTime) { }

        public virtual void Rotate(Quaternion rotation, float rotateSpeed, float deltaTime) { }

        public virtual void Jump(float impulse) { }

        public virtual void SwitchMoveType(MoveType moveType)
        {

        }
    }
}
