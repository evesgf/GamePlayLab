using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPL
{
    public class BaseCharacterController : MonoBehaviour
    {
        #region EDITOR EXPOSED FIELDS

        #endregion

        #region FIELDS
        private Vector3 _moveDireciton;
        private bool _jump;
        private bool _canJump=true;
        private bool _isJumping;
        #endregion

        #region PROPERTIES
        /// <summary>
        /// 缓存运动控制器
        /// </summary>
        public CharacterMovement movement { get; private set; }

        /// <summary>
        /// 运动输入命令，期望的运动方向
        /// </summary>
        public Vector3 moveDirction
        {
            get { return _moveDireciton; }
            set { _moveDireciton = Vector3.ClampMagnitude(value,1.0f); }
        }

        /// <summary>
        /// 跳跃命令
        /// </summary>
        public bool jump
        {
            get { return _jump; }
            set
            {
                if (_jump && value == false)
                {
                    _canJump = true;
                }
                _jump = value;
            }
        }

        public bool isJumping
        {
            get
            {
                // We are in jump mode but just falling

                if (_isJumping && movement.velocity.y < 0.0001f)
                    _isJumping = false;

                return _isJumping;
            }
        }
        #endregion

        #region METHODS
        /// <summary>
        /// Handles Input
        /// </summary>
        protected virtual void HandleInput()
        {
            moveDirction = new Vector3
            {
                x = Input.GetAxisRaw("Horizontal"),
                y = 0.0f,
                z=Input.GetAxisRaw("Vertical")
            };

            jump = Input.GetButton("Jump");
        }

        /// <summary>
        /// 执行运动逻辑
        /// NOTE:必须在FixedUpdate执行
        /// </summary>
        protected void Move()
        {
            //移动逻辑，矢量运动或RootMotion

            //跳跃逻辑

            //RootMotion状态更新
        }
        #endregion

        #region MONOBEHAVIOUR

        public virtual void Awake()
        {
            movement = GetComponent<CharacterMovement>();
        }

        public virtual void FixedUpdate()
        {
            // Perform character movement
            Move();
        }

        public virtual void Update()
        {
            // Handle input
            HandleInput();
        }

        #endregion
    }
}
