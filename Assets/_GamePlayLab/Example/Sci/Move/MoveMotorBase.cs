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
        // Start is called before the first frame update
        public virtual void Start() { }

        public virtual void Update() { }


        public virtual void Move(Vector3 moveDir, float moveSpeed,float deltaTime) { }

        public virtual void Rotate(Vector3 moveDir, float rotateSpeed, float deltaTime) { }

        public virtual void SwitchMoveType(MoveType moveType)
        {

        }
    }
}
