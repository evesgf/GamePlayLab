using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPL
{
    public class FlyState : StateBase
    {
        [Header("Normal Fly")]
        public moveType moveType = moveType.MoveToCamera;

        public Transform cam;

        public float moveAniSpeed = 1.0f;
        public float moveSpeed;
        public float moveDrag = 0.1f;
        public float moveRotateSpeed;

        [Header("Sprint Fly")]
        public float sprintAniSpeed = 1.0f;
        public float sprintSpeed;
        public float sprintDrag = 0.1f;
        public float sprintRotateSpeed;

        public float sprintStopDuration = 0.5f;
        public AnimationCurve sprintStopCurve;

        private StateMachine FSM;
        private PlayerController playerController;

        private Vector3 camForward;

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

            for (float i = 0; i < sprintStopDuration; i += Time.deltaTime)
            {
                //沿当前刚体移动方向继续移动
                playerController.movement.Move(moveDir, sprintStopCurve.Evaluate(i / sprintStopDuration) * sprintSpeed, i);
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
            this.name = "FlyState";
        }

        public override int GetStateID()
        {
            return (int)PlayerState.Fly;
        }

        public override void OnEnter(StateMachine stateMachine, IState prevState, object param1, object param2)
        {
            base.OnEnter(stateMachine, prevState, param1, param2);

            playerController.movement.useGravity = false;

            playerController.aniSpeed = moveAniSpeed;

        }

        public override void OnLeave(IState nextState, object param1, object param2)
        {
            playerController.movement.useGravity = true;

            base.OnLeave(nextState, param1, param2);
        }

        public override void OnFixedUpdate(float elapseSeconds, float realElapseSeconds)
        {
            isSprint = playerController.isSprinting;

            playerController.aniSpeed = isSprint ? sprintAniSpeed : moveAniSpeed;

            switch (moveType)
            {
                case moveType.MoveToForward:
                    playerController.realMoveDirection = Vector3.MoveTowards(playerController.realMoveDirection, playerController.currentMoveDirection.z * Vector3.forward + playerController.currentMoveDirection.x * Vector3.right, isSprint ? sprintDrag : moveDrag);
                    break;

                case moveType.MoveToCamera:
                    //计算视口方向
                    camForward = Vector3.Scale(cam.forward, new Vector3(1, 0, 1)).normalized;
                    playerController.realMoveDirection = Vector3.MoveTowards(playerController.realMoveDirection, playerController.currentMoveDirection.z * camForward + playerController.currentMoveDirection.x * cam.right, moveDrag);

                    //朝向视口
                    playerController.movement.Rotate(cam.forward, moveRotateSpeed, elapseSeconds);
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