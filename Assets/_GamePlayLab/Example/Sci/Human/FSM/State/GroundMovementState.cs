using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPL
{
    public class GroundMovementState : StateBase
    {
        public float moveSpeed;
        public float rotateSpeed;

        private StateMachine FSM;
        private PlayerController playerController;

        private void Start()
        {
            FSM = GetComponentInParent<StateMachine>();
            playerController = FSM.m_owner.GetComponent<PlayerController>();
            FSM.RegistState(this);
            this.name = "GroundMovementState";
        }

        public override int GetStateID()
        {
            return (int)PlayerState.GroundMovement;
        }

        public override void OnFixedUpdate(float elapseSeconds, float realElapseSeconds)
        {
            playerController.realMoveDirection = playerController.currentMoveDirection.z * Vector3.forward + playerController.currentMoveDirection.x * Vector3.right;

            //移动
            playerController._rigidbody.MovePosition(playerController.transform.position + playerController.realMoveDirection * moveSpeed * elapseSeconds);

            //旋转
            playerController._rigidbody.MoveRotation (Quaternion.Slerp(playerController.transform.rotation, Quaternion.LookRotation(playerController.realMoveDirection), Time.deltaTime * rotateSpeed));
        }

        public override void OnLateUpdate(float elapseSeconds, float realElapseSeconds)
        {

        }

        public override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {

        }
    }
}
