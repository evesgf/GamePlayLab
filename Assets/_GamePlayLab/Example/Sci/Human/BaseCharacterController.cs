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
        [SerializeField]
        private float _angularSpeed = 540.0f;
        [Header("Jump")]
        [Tooltip("The initial jump height (in meters).")]
        [SerializeField]
        private float _baseJumpHeight = 1.5f;
        [Tooltip("Maximum turning speed (in deg/s).")]

        [Header("Avatar")]
        [SerializeField]
        private GameObject _avatar;
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

        /// <summary>
        /// Cached animator component (if any).
        /// </summary>

        public Animator animator { get; set; }

        /// <summary>
        /// Maximum turning speed (in deg/s).
        /// </summary>

        public float angularSpeed
        {
            get { return _angularSpeed; }
            set { _angularSpeed = Mathf.Max(0.0f, value); }
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

        public void RotateTowardsMoveDirection(bool onlyLateral = true)
        {
            RotateTowards(moveDirection, onlyLateral);
        }
        /// <summary>
        /// Rotate the character towards a given direction vector.
        /// </summary>
        /// <param name="direction">The target direction</param>
        /// <param name="onlyLateral">Should it be restricted to XZ only?</param>

        public void RotateTowards(Vector3 direction, bool onlyLateral = true)
        {
            movement.Rotate(direction, angularSpeed, onlyLateral);
        }

        protected void Jump()
        {
            if (!_jump || !_canJump) return;
            //if (!movement.isGrounded) return;

            _canJump = false;           // Halt jump until jump button is released
            _isJumping = true;          // Update isJumping flag

            movement.ApplyVerticalImpulse(jumpImpulse);
        }

        protected virtual void UpdateRotation()
        {
            // Rotate towards movement direction (input)

            RotateTowardsMoveDirection();
        }

        public virtual void Animate()
        {
            if (animator == null) return;

            var move = transform.InverseTransformDirection(moveDirection);
            var forwardAmount = move.z;

            animator.SetFloat("MoveSpeed", forwardAmount, 0.1f, Time.deltaTime);
            animator.SetBool("OnGround", movement.isGrounded);

            if (!movement.isGrounded)
                animator.SetFloat("Jump", movement.velocity.y, 0.1f, Time.deltaTime);
        }
        #endregion

        #region MONOBEHAVIOUR

        public virtual void Awake()
        {
            movement = GetComponent<CharacterMovement>();
            animator = _avatar.GetComponent<Animator>();
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

            // Update character rotation
            UpdateRotation();

            // Perform character animation
            Animate();
        }

        #endregion
    }
}
