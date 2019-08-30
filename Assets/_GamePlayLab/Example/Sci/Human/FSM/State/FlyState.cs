﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPL
{
    public class FlyState : StateBase
    {
        public Transform cam;

        [Header("Normal Fly")]
        public moveType normalMoveType = moveType.MoveToForward;

        public float normalBlur = 0;
        public float sprintBlur = 0.007f;
        public float blurTransition = 5f;
        private RadialBlurEffect radialBlurEffect;

        public float moveAniSpeed = 1.0f;
        public float moveSpeed;
        public float moveDrag = 0.1f;
        public float moveRotateSpeed;

        [Header("Sprint Fly")]
        public moveType sprintMoveType = moveType.MoveToCamera;
        public float sprintAniSpeed = 1.0f;
        public float sprintSpeed;
        public float sprintDrag = 0.1f;
        public float sprintRotateSpeed;
        public PlayerTrail trail;

        public float sprintStopDuration = 0.5f;
        public AnimationCurve sprintStopCurve;

        private StateMachine FSM;
        private PlayerController playerController;

        private Vector3 camForward;

        private moveType moveType;
        private bool isSprint;
        private bool isSprintStop;
        private bool isSprintStopTimer;

        #region METHODS
        IEnumerator OnSprintStop()
        {
            trail.CloseTrail();

            isSprintStop = true;
            playerController.isSprinting = false;
            var moveDir = playerController.movement.velocity.normalized;
            playerController.realMoveDirection = Vector3.zero;

            //设置动画
            playerController.isSprintStop = true;

            for (float i = 0; i < sprintStopDuration; i += Time.deltaTime)
            {
                //沿当前刚体移动方向继续移动
                playerController.movement.AirMove(moveDir, sprintStopCurve.Evaluate(i / sprintStopDuration) * sprintSpeed, i);
                yield return i;
            }
            isSprintStop = false;

            trail.OpenTrail();
        }
        #endregion

        private void Start()
        {
            radialBlurEffect = cam.GetComponent<RadialBlurEffect>();

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

            trail.OpenTrail();
        }

        public override void OnLeave(IState nextState, object param1, object param2)
        {
            playerController.movement.useGravity = true;

            trail.CloseTrail();

            base.OnLeave(nextState, param1, param2);
        }

        public override void OnFixedUpdate(float elapseSeconds, float realElapseSeconds)
        {
            isSprint = playerController.isSprinting;
            //切换Trail和屏幕模糊
            if (isSprint)
            {
                trail.TrailHigh();
                radialBlurEffect.blurFactor = Mathf.Lerp(radialBlurEffect.blurFactor, sprintBlur, blurTransition * Time.deltaTime);
            }
            else
            {
                trail.TrailLow();
                radialBlurEffect.blurFactor = Mathf.Lerp(radialBlurEffect.blurFactor, normalBlur, blurTransition * Time.deltaTime);
            }

            playerController.aniSpeed = isSprint ? sprintAniSpeed : moveAniSpeed;

            if (!isSprintStop)
            {
                moveType = isSprint ? sprintMoveType : normalMoveType;
                //计算视口方向
                camForward = Vector3.Scale(cam.forward, new Vector3(1, 0, 1)).normalized;
                switch (moveType)
                {
                    case moveType.MoveToForward:
                        playerController.realMoveDirection = Vector3.MoveTowards(playerController.realMoveDirection, playerController.currentMoveDirection.z * camForward + playerController.currentMoveDirection.x * cam.right + playerController.currentMoveDirection.y * Vector3.up, isSprint ? sprintDrag : moveDrag);

                        //朝向视口
                        if (playerController.currentMoveDirection.x != 0 || playerController.currentMoveDirection.z != 0)
                        {
                            playerController.movement.GroundRotate(playerController.realMoveDirection, moveRotateSpeed, elapseSeconds);
                        };

                        //锁定Animaotr的Horizontal输入防止切换横向动画
                        playerController.LockHorizontal = true;
                        break;

                    case moveType.MoveToCamera:
                        playerController.realMoveDirection = Vector3.MoveTowards(playerController.realMoveDirection, playerController.currentMoveDirection.z * camForward + playerController.currentMoveDirection.x * cam.right + playerController.currentMoveDirection.y * Vector3.up, moveDrag);

                        //朝向视口
                        playerController.movement.AirRotate(cam.forward, moveRotateSpeed, elapseSeconds);

                        //开启Animaotr的Horizontal输入切换横向动画
                        playerController.LockHorizontal = false;
                        break;

                    case moveType.MoveToTarget:
                        break;

                    default:
                        break;
                }
            }

            //SprintStop状态检测
            if ((moveType == moveType.MoveToCamera ? playerController.currentMoveDirection.z < float.Epsilon : playerController.currentMoveDirection == Vector3.zero) && isSprint && isSprintStop == false)
            {
                StartCoroutine(OnSprintStop());
            }
            else
            {
                if (!isSprintStop)
                {
                    //移动
                    playerController.movement.AirMove(playerController.realMoveDirection, isSprint ? sprintSpeed : moveSpeed, elapseSeconds);
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