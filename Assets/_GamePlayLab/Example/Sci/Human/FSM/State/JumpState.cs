using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPL
{
    public class JumpState : StateBase
    {
        [Header("Jump")]
        public moveType moveType = moveType.MoveToForward;

        public float moveAniSpeed = 1.0f;

        public float baseJumpHeight = 1.5f;
        public float extraJumpTime = 0.5f;
        public float extraJumpPower = 25f;
        public float jumpToleranceTime = 0.15f;
        public float maxMidAirJumps = 1;

        public float moveSpeed;
        public float drag = 0.1f;
        public float rotateSpeed;

        private StateMachine FSM;
        private PlayerController playerController;

        private float onEnterTime = 0;

        #region PROPERTIES
        public float jumpImpulse
        {
            get { return Mathf.Sqrt(2.0f * baseJumpHeight * playerController.movement.gravity); }
        }
        #endregion

        #region METHODS

        #endregion

        private void Start()
        {
            FSM = GetComponentInParent<StateMachine>();
            playerController = FSM.m_owner.GetComponent<PlayerController>();
            FSM.RegistState(this);
            this.name = "JumpState";
        }

        public override int GetStateID()
        {
            return (int)PlayerState.Jump;
        }

        public override void OnEnter(StateMachine stateMachine, IState prevState, object param1, object param2)
        {
            base.OnEnter(stateMachine, prevState, param1, param2);

            playerController.aniSpeed = moveAniSpeed;

            playerController.movement.ApplyVerticalImpulse(jumpImpulse);

            onEnterTime = 0;
        }

        public override void OnFixedUpdate(float elapseSeconds, float realElapseSeconds)
        {
            onEnterTime += elapseSeconds;

            if (playerController.groundDetection.isOnGround && onEnterTime> jumpToleranceTime)
            {
                FSM.SwitchState((int)PlayerState.GroundMovement, null, null);
            }

            playerController.aniSpeed = moveAniSpeed;

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
                playerController.movement.Rotate(playerController.currentMoveDirection, rotateSpeed, elapseSeconds);
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