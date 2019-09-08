using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPL
{
    public class WallMovementState : StateBase
    {
        public MovementType moveType = MovementType.MoveToForward;

        public float moveAniSpeed = 1.2f;
        public float moveSpeed=5;
        public float drag = 0.1f;
        public float rotateSpeed=8f;

        private StateMachine FSM;
        private PlayerController playerController;

        private void Start()
        {
            FSM = GetComponentInParent<StateMachine>();
            playerController = FSM.m_owner.GetComponent<PlayerController>();
            FSM.RegistState(this);
            this.name = "WallMovementState";
        }

        public override int GetStateID()
        {
            return (int)PlayerState.WallMovement;
        }

        public override void OnEnter(StateMachine stateMachine, IState prevState, object param1, object param2)
        {
            base.OnEnter(stateMachine, prevState, param1, param2);
            playerController.isWall = true;

            playerController.movement.useGravity = false;
            //playerController.movement.velocity = Vector3.zero;

            playerController.LockHorizontal = false;

            //旋转角色贴合墙面
            playerController.transform.rotation= Quaternion.AngleAxis(-90f, Vector3.right)*playerController.transform.rotation;
        }

        public override void OnLeave(IState nextState, object param1, object param2)
        {
            playerController.movement.useGravity = true;

            playerController.isWall = false;
            base.OnLeave(nextState, param1, param2);
        }

        public override void OnFixedUpdate(float elapseSeconds, float realElapseSeconds)
        {
            //触墙检测
            if (playerController.groundDetection.isOnGround)
            {
                playerController.movement.useGravity = false;
            }

            switch (moveType)
            {
                case MovementType.MoveToForward:
                    playerController.realMoveDirection = Vector3.MoveTowards(playerController.realMoveDirection, playerController.currentMoveDirection.z * playerController.transform.forward + playerController.currentMoveDirection.x * Vector3.right, drag);
                    break;

                case MovementType.MoveToCamera:
                    break;

                case MovementType.MoveToTarget:
                    break;

                default:
                    break;
            }

            playerController.aniSpeed = moveAniSpeed;

            playerController.movement.GroundMove(playerController.realMoveDirection, moveSpeed, elapseSeconds,playerController.groundDetection.surfaceNormal, playerController.groundDetection.surfaceNormal.normalized);

            //if (playerController.currentMoveDirection != Vector3.zero)
            //{
            //    playerController.movement.GroundRotate(playerController.currentMoveDirection, rotateSpeed, elapseSeconds);
            //}
        }

        public override void OnLateUpdate(float elapseSeconds, float realElapseSeconds)
        {

        }

        public override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {

        }
    }
}
