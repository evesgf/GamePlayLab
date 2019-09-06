using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPL
{
    public enum ClimbOberObsType
    {
        None=0,
        Low=1,
        High=2
    }

    [System.Serializable]
    public class ClimbOverObsProperty
    {
        public ClimbOberObsType climbOberObsType;
        public float moveAniSpeed = 1.0f;
        public float climbUpTime = 0.5f;
        public float climbDownTime = 0.5f;

        //第一段位移终点的上下偏移量
        public float climbCenterHighOffest = 0;

        //设置动画状态机用以切换翻越动画
        public float ani_ClimbOverObsHigh = 0;
    }

    public class ClimbOverObsState : StateBase
    {

        public ClimbOverObsProperty[] climbOverObs;

        private StateMachine FSM;
        private PlayerController playerController;

        private void Start()
        {
            FSM = GetComponentInParent<StateMachine>();
            playerController = FSM.m_owner.GetComponent<PlayerController>();
            FSM.RegistState(this);
            this.name = "ClimbOverObsState";
        }

        public override int GetStateID()
        {
            return (int)PlayerState.ClimbOverObs;
        }

        public override void OnEnter(StateMachine stateMachine, IState prevState, object param1, object param2)
        {
            base.OnEnter(stateMachine, prevState, param1, param2);

            playerController.movement.velocity = Vector3.zero;
            playerController.movement.useGravity = false;

            var nowClimbOverObs = new ClimbOverObsProperty();
            foreach (var item in climbOverObs)
            {
                if (item.climbOberObsType == playerController.wallDetection.climbOberObsType)
                    nowClimbOverObs = item;
            }
            print(nowClimbOverObs.climbOberObsType);
            playerController.aniSpeed = nowClimbOverObs.moveAniSpeed;
            playerController._avatar.SetFloat("ClimbOverObsHigh", nowClimbOverObs.ani_ClimbOverObsHigh);

            playerController.isClimbOverObs = true;

            var a = playerController.wallDetection.ClimbCenterWithHighOffset(nowClimbOverObs.climbCenterHighOffest);
            var b = playerController.wallDetection.climbEnd;

            var tweener = playerController.transform.DOMove(a,nowClimbOverObs.climbUpTime).OnComplete(() =>
            {
                playerController.transform.DOMove(b, nowClimbOverObs.climbDownTime).OnComplete(()=> {
                    FSM.SwitchState((int)PlayerState.GroundMovement, null, null);
                });
            });
        }

        public override void OnLeave(IState nextState, object param1, object param2)
        {
            playerController.movement.useGravity = true;
            playerController.isClimbOverObs = false;
            base.OnLeave(nextState, param1, param2);
        }

        public override void OnFixedUpdate(float elapseSeconds, float realElapseSeconds)
        {

        }

        public override void OnLateUpdate(float elapseSeconds, float realElapseSeconds)
        {

        }

        public override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {

        }
    }
}
