using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPL
{
    public class GroundMovementState : StateBase
    {
        [Header("Normal Run")]
        public moveType moveType = moveType.MoveToForward;

        public float moveAniSpeed=1.0f;
        public float moveSpeed;
        public float moveDrag=0.1f;
        public float moveRotateSpeed;

        [Header("Sprint Run")]
        public float sprintAniSpeed = 1.0f;
        public float sprintSpeed;
        public float sprintDrag = 0.1f;
        public float sprintRotateSpeed;

        public float sprintStopDuration=0.5f;
        public AnimationCurve sprintStopCurve;

        private StateMachine FSM;
        private PlayerController playerController;

        private bool isSprint;
        private bool isSprintStop;
        private bool isSprintStopTimer;

        #region METHODS
        IEnumerator OnSprintStop()
        {
            isSprintStop = true;
            playerController.isSprinting = false;
            var moveDir = playerController.movement.velocity.normalized;

            //设置动画
            playerController.isSprintStop = true;

            for (float i = 0; i < sprintStopDuration; i+=Time.deltaTime)
            {
                //沿当前刚体移动方向继续移动
                playerController.movement.Move(moveDir, sprintStopCurve.Evaluate(i / sprintStopDuration) * sprintSpeed ,i);
                yield return i;
            }
            isSprintStop = false;
        }
        #endregion

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

        public override void OnEnter(StateMachine stateMachine, IState prevState, object param1, object param2)
        {
            base.OnEnter(stateMachine, prevState, param1, param2);

            playerController.aniSpeed = moveAniSpeed;

            isSprintStop = false;
        }

        public override void OnFixedUpdate(float elapseSeconds, float realElapseSeconds)
        {
            isSprint = playerController.isSprinting;

            playerController.aniSpeed = isSprint? sprintAniSpeed:moveAniSpeed;

            switch (moveType)
            {
                case moveType.MoveToForward:
                    playerController.realMoveDirection = Vector3.MoveTowards(playerController.realMoveDirection, playerController.currentMoveDirection.z * Vector3.forward + playerController.currentMoveDirection.x * Vector3.right, isSprint? sprintDrag: moveDrag);
                    break;

                case moveType.MoveToCamera:
                    break;

                case moveType.MoveToTarget:
                    break;

                default:
                    break;
            }

            //SprintStop状态检测
            if (playerController.currentMoveDirection == Vector3.zero && isSprint && isSprintStop == false)
            {
                StartCoroutine(OnSprintStop());
            }
            else
            {
                if (!isSprintStop)
                {
                    //移动
                    playerController.movement.Move(playerController.realMoveDirection, isSprint ? sprintSpeed : moveSpeed, elapseSeconds);

                    //旋转
                    if (playerController.currentMoveDirection != Vector3.zero)
                    {
                        playerController.movement.Rotate(playerController.currentMoveDirection, isSprint ? sprintRotateSpeed : moveRotateSpeed, elapseSeconds);
                    }
                }
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
