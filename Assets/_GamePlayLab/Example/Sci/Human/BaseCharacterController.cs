using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPL
{
    public class BaseCharacterController : MonoBehaviour
    {
        #region EDITOR EXPOSED FIELDS
        [Header("Movement")]
        [Tooltip("Maximum movement speed (in m/s).")]
        [SerializeField]
        private float _speed = 5.0f;
        [Tooltip("The rate of change of velocity.")]
        [SerializeField]
        private float _acceleration = 50.0f;
        [Tooltip("The rate at which the character's slows down.")]
        [SerializeField]
        private float _deceleration = 20.0f;
        [Header("Jump")]
        [Tooltip("The initial jump height (in meters).")]
        [SerializeField]
        private float _baseJumpHeight = 1.5f;
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
        public Vector3 moveDirection
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

        /// <summary>
        /// The rate of change of velocity.
        /// </summary>
        public float acceleration
        {
            get { return _acceleration; }
            set { _acceleration = Mathf.Max(0.0f, value); }
        }
        /// <summary>
        /// The rate at which the character's slows down.
        /// </summary>

        public float deceleration
        {
            get { return _deceleration; }
            set { _deceleration = Mathf.Max(0.0f, value); }
        }

        /// <summary>
        /// Calculate the desired movement velocity.
        /// Eg: Convert the input (moveDirection) to movement velocity vector,
        ///     use navmesh agent desired velocity, etc.
        /// </summary>

        protected virtual Vector3 CalcDesiredVelocity()
        {
            return moveDirection * speed;
        }

        /// <summary>
        /// Maximum movement speed (in m/s).
        /// </summary>

        public float speed
        {
            get { return _speed; }
            set { _speed = Mathf.Max(0.0f, value); }
        }

        /// <summary>
        /// The initial jump height (in meters).
        /// </summary>

        public float baseJumpHeight
        {
            get { return _baseJumpHeight; }
            set { _baseJumpHeight = Mathf.Max(0.0f, value); }
        }

        /// <summary>
        /// Computed jump impulse.
        /// </summary>

        public float jumpImpulse
        {
            get { return Mathf.Sqrt(2.0f * baseJumpHeight * movement.gravity); }
        }
        #endregion

        #region METHODS
        /// <summary>
        /// Handles Input
        /// </summary>
        protected virtual void HandleInput()
        {
            moveDirection = new Vector3
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
            var desiredVelocity = CalcDesiredVelocity();
            //移动逻辑，矢量运动或RootMotion
            movement.Move(desiredVelocity,speed, acceleration,deceleration);
            //跳跃逻辑

            //RootMotion状态更新
        }

        protected void Jump()
        {
            if (!_jump || !_canJump) return;
            //if (!movement.isGrounded) return;

            _canJump = false;           // Halt jump until jump button is released
            _isJumping = true;          // Update isJumping flag

            movement.ApplyVerticalImpulse(jumpImpulse);
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
            Jump();
        }

        public virtual void Update()
        {
            // Handle input
            HandleInput();
        }

        #endregion
    }
}
