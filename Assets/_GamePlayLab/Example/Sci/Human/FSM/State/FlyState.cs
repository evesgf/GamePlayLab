using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPL
{
    public class FlyState : StateBase
    {
        [Header("Fly")]
        public moveType moveType = moveType.MoveToCamera;

        public Transform cam;

        public float moveAniSpeed = 1.0f;
        public float moveSpeed;
        public float drag = 0.1f;
        public float rotateSpeed;

        private StateMachine FSM;
        private PlayerController playerController;

        private Vector3 camForward;

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
            playerController.aniSpeed = moveAniSpeed;


            switch (moveType)
            {
                case moveType.MoveToForward:
                    playerController.realMoveDirection = Vector3.MoveTowards(playerController.realMoveDirection, playerController.currentMoveDirection.z * Vector3.forward + playerController.currentMoveDirection.x * Vector3.right, drag);
                    break;

                case moveType.MoveToCamera:
                    //计算视口方向
                    camForward = Vector3.Scale(cam.forward, new Vector3(1, 0, 1)).normalized;
                    playerController.realMoveDirection = Vector3.MoveTowards(playerController.realMoveDirection, playerController.currentMoveDirection.z * camForward + playerController.currentMoveDirection.x * cam.right,drag);

                    //朝向视口
                    playerController.movement.Rotate(cam.forward, rotateSpeed, elapseSeconds);
                    break;

                case moveType.MoveToTarget:
                    break;

                default:
                    break;
            }

            //移动
            playerController.movement.Move(playerController.realMoveDirection, moveSpeed, elapseSeconds);
        }

        public override void OnLateUpdate(float elapseSeconds, float realElapseSeconds)
        {

        }

        public override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {

        }
    }
}