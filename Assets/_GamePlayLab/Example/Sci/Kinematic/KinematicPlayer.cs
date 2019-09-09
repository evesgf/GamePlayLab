using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPL.KC
{
    public enum MovementType
    {
        MoveToForward = 0,
        MoveToCamera = 1,
        MoveToTarget = 2
    }

    public class KinematicPlayer : MonoBehaviour
    {
        [Header("Movement Type")]
        public Transform cam;
        public MovementType movementType = MovementType.MoveToForward;

        [Header("Plane Movement")]
        public float maxMoveSpeed = 10f;
        public float moveAcceleratedSpeed = 15f;
        public float moveDamp = 10f;

        public KinematicMovement movement;
        public KinematicGroundDetection groundDetection;

        //Input
        private Vector3 _currentMoveDireciton;
        public Vector3 currentMoveDirection
        {
            get { return _currentMoveDireciton; }
            set { _currentMoveDireciton = GetCurrentMoveDirecitonFromMovementType(Vector3.ClampMagnitude(value, 1.0f)); }
        }

        #region METHOD
        /// <summary>
        /// 设置运动向量
        /// </summary>
        /// <param name="velocity"></param>
        /// <param name="deltaTime"></param>
        public void ApplyVelocity(Vector3 velocity, float deltaTime)
        {
            float currentVelocityMagnitude = velocity.magnitude;

            //运动检查
            if (velocity.magnitude < 0.01f) velocity = Vector3.zero;
        }

        /// <summary>
        /// 根据移动类型返回对应的移动向量
        /// </summary>
        /// <param name="moveDir"></param>
        /// <returns></returns>
        public Vector3 GetCurrentMoveDirecitonFromMovementType(Vector3 moveDir)
        {
            //计算视口方向
            var camForward = Vector3.Scale(cam.forward, new Vector3(1, 0, 1)).normalized;
            switch (movementType)
            {
                case MovementType.MoveToForward:
                    moveDir = moveDir.z * camForward + moveDir.x * cam.right + moveDir.y * -movement.gravityDir;
                    break;

                case MovementType.MoveToCamera:
                    moveDir = moveDir.z * camForward + moveDir.x * cam.right + moveDir.y * -movement.gravityDir;
                    break;

                case MovementType.MoveToTarget:
                    break;

                default:
                    break;
            }

            return moveDir;
        }
        #endregion

        // Start is called before the first frame update
        void Awake()
        {
            movement = GetComponent<KinematicMovement>();
            movement.player = this;
            groundDetection = GetComponent<KinematicGroundDetection>();
            groundDetection.movement = movement;
        }

        private void Start()
        {
            
        }


        // Update is called once per frame
        void Update()
        {
            //Update Input
            currentMoveDirection = new Vector3
            {
                x = Input.GetAxisRaw("Horizontal"),
                y = Input.GetAxisRaw("UpAxis"),
                z = Input.GetAxisRaw("Vertical")
            };
        }

        private void FixedUpdate()
        {
            groundDetection.DetectGround();

            if (groundDetection.isOnGround)
            {
                movement.ApplyGroundMovement(currentMoveDirection* maxMoveSpeed, maxMoveSpeed, moveAcceleratedSpeed, moveDamp);
            }
            else
            {
                movement.ApplyAirMovement(currentMoveDirection*maxMoveSpeed, maxMoveSpeed, moveAcceleratedSpeed, moveDamp);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, transform.position + currentMoveDirection);
        }
    }
}
