using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPL
{
    public class FallState : StateBase
    {
        public moveType moveType = moveType.MoveToForward;
        public float drag = 0.1f;
        public float rotateSpeed;

        private StateMachine FSM;
        private PlayerController playerController;

        private void Start()
        {
            FSM = GetComponentInParent<StateMachine>();
            playerController = FSM.m_owner.GetComponent<PlayerController>();
            FSM.RegistState(this);
            this.name = "FallState";
        }

        public override int GetStateID()
        {
            return (int)PlayerState.Fall;
        }

        public override void OnEnter(StateMachine stateMachine, IState prevState, object param1, object param2)
        {
            base.OnEnter(stateMachine, prevState, param1, param2);
            playerController.fall = true;
        }

        public override void OnLeave(IState nextState, object param1, object param2)
        {
            playerController.fall = false;
            base.OnLeave(nextState, param1, param2);
        }

        public override void OnFixedUpdate(float elapseSeconds, float realElapseSeconds)
        {
            //触地检测
            if (playerController.groundDetection.isOnGround)
            {
                FSM.SwitchState((int)PlayerState.GroundMovement, null, null);
            }

            switch (moveType)
            {
                case moveType.MoveToForward:
                    playerController.realMoveDirection = Vector3.MoveTowards(playerController.realMoveDirection, playerController.currentMoveDirection.z * Vector3.forward + playerController.currentMoveDirection.x * Vector3.right, drag);
                    break;

                case moveType.MoveToCamera:
                    break;

                case moveType.MoveToTarget:
                    break;

                default:
                    break;
            }

            //playerController.movement.Move(playerController.realMoveDirection, moveSpeed, elapseSeconds);

            if (playerController.currentMoveDirection != Vector3.zero)
            {
                playerController.movement.GroundRotate(playerController.currentMoveDirection, rotateSpeed, elapseSeconds);
            }
        }

        public override void OnLateUpdate(float elapseSeconds, float realElapseSeconds)
        {

        }

        public override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {

        }
    }
}
