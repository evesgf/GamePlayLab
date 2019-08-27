using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPL
{
    /// <summary>
    /// 角色移动类型
    /// </summary>
    public enum moveType
    {
        MoveToForward = 0,
        MoveToCamera = 1,
        MoveToTarget = 2
    }

    public class PlayerMovement : MonoBehaviour
    {
        #region EDITOR EXPOSED FIELDS
        [Tooltip("启用自定义引力")]
        [SerializeField]
        private bool _useGravity = true;
        [SerializeField]
        private float _gravity = 9.8f;
        #endregion

        #region FIELDS

        #endregion

        #region PROPERTIES
        public PlayerController playerController{ get; private set; }
        public bool useGravity
        {
            get { return _useGravity; }
            set { _useGravity = value; }
        }

        public float gravity
        {
            get { return _gravity; }
            set { _gravity = Mathf.Max(0.0f, value); }
        }

        /// <summary>
        /// 角色本身的速度矢量
        /// </summary>
        public Vector3 velocity
        {
            get { return playerController._rigidbody.velocity; }
            set { playerController._rigidbody.velocity = value; }
        }
        #endregion

        #region METHODS
        //移动
        public void Move(Vector3 desiredVelocity,float moveSpeed,float elapseSeconds)
        {
            desiredVelocity = GetTangent(desiredVelocity, Vector3.up, Vector3.up) * Mathf.Min(desiredVelocity.magnitude*moveSpeed, moveSpeed);

            velocity = desiredVelocity;

            if (useGravity)
            {
                if (playerController.groundDetection.isOnGround)
                {
                    velocity += Vector3.down * gravity * elapseSeconds;
                }
                else
                {
                    velocity += Vector3.down * gravity;
                }
            }

            //playerController._rigidbody.MovePosition(playerController.transform.position + playerController.realMoveDirection * moveSpeed * elapseSeconds);
        }

        //旋转
        public void Rotate(Vector3 desiredVelocity, float rotateSpeed, float elapseSeconds)
        {
            playerController._rigidbody.MoveRotation(Quaternion.Slerp(playerController.transform.rotation, Quaternion.LookRotation(desiredVelocity), Time.deltaTime * rotateSpeed));
        }

        /// <summary>
        /// 返回与表面相切的法向量
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="normal"></param>
        /// <param name="up">？</param>
        /// <returns></returns>
        public Vector3 GetTangent(Vector3 direction, Vector3 normal, Vector3 up)
        {
            var right = Vector3.Cross(direction, up);
            var tangent = Vector3.Cross(normal, right);

            return tangent.normalized;
        }

        /// <summary>
        /// 添加一个向上的脉冲，比如跳跃
        /// </summary>
        /// <param name="impulse"></param>
        public void ApplyVerticalImpulse(float impulse)
        {
            var verticalImpulse = Vector3.up * impulse;
            var v=playerController.movement.velocity;
            v.y = 0;
            playerController.movement.velocity = v + verticalImpulse;
        }

        /// <summary>
        /// 添加额外力
        /// </summary>
        /// <param name="force"></param>
        /// <param name="forceMode"></param>
        public void ApplyForce(Vector3 force, ForceMode forceMode = ForceMode.Force)
        {
            playerController._rigidbody.AddForce(force, forceMode);
        }
        #endregion

        // Start is called before the first frame update
        void Start()
        {
            playerController = GetComponent<PlayerController>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void FixedUpdate()
        {
            playerController.groundDetection.DetectGround();

            if (useGravity)
            {
                velocity += Vector3.down * gravity * Time.fixedDeltaTime;
            }
        }
    }

}