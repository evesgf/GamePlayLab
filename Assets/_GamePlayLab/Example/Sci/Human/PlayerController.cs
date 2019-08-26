using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPL
{
    public class PlayerController : MonoBehaviour
    {
        public BaseGroundDetection groundDetection;

        public StateMachine FSM;

        #region FIELDS
        public Avatar _avatar;
        public float _aniSpeed=1.0f;
        internal Rigidbody _rigidbody;
        private Vector3 _currentMoveDireciton;
        private Vector3 _realMoveDirection;

        private bool _jump;
        private bool _canJump=true;
        private bool _isJumping;
        private float _jumpButtonHeldDownTimer;

        private bool _fly;
        #endregion

        #region PROPERTIES
        public Animator animator { get; set; }

        /// <summary>
        /// 动画运动速度
        /// </summary>
        public float aniSpeed { get; set; }

        public PlayerMovement movement { get; private set; }

        /// <summary>
        /// 运动输入命令，期望的运动方向
        /// </summary>
        /// 
        public Vector3 currentMoveDirection
        {
            get { return _currentMoveDireciton; }
            set { _currentMoveDireciton = Vector3.ClampMagnitude(value, 1.0f); }
        }

        /// <summary>
        /// 角色真实运动方向
        /// </summary>
        public Vector3 realMoveDirection
        {
            get { return _realMoveDirection; }
            set { _realMoveDirection = Vector3.ClampMagnitude(value, 1.0f); }
        }

        public bool jump
        {
            get { return _jump; }
            set
            {
                if (_jump && value == false)
                {
                    _canJump = true;
                    _jumpButtonHeldDownTimer = 0.0f;
                }
                _jump = value;
                if (_jump) _jumpButtonHeldDownTimer += Time.deltaTime;
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

        public bool fly
        {
            get { return _fly; }
            set
            {
                if (!_fly && value == true)
                {
                    if (!groundDetection.isOnGround)
                    {
                        FSM.SwitchState((int)PlayerState.Fly, null, null);
                        _fly = value;
                    } 
                }
            }
        }
        #endregion

        #region Methold
        private void UpdateAnimator()
        {
            var move = transform.InverseTransformDirection(realMoveDirection);

            _avatar.SetFloat("AniSpeed", aniSpeed);
            _avatar.SetFloat("MoveSpeed", move.z, 0.1f, Time.deltaTime);
            _avatar.SetBool("OnGround", groundDetection.isOnGround);
            _avatar.SetFloat("Jump",movement.velocity.y);

            _avatar.SetFloat("Horizontal", currentMoveDirection.x, 0.1f, Time.deltaTime);
            _avatar.SetFloat("Vertical", currentMoveDirection.z, 0.1f, Time.deltaTime);

            _avatar.SetBool("IsFly", _fly);
        }

        /// <summary>
        /// 跳跃状态检查
        /// </summary>
        public void CheckJump()
        {
            //如果没有抬起跳跃键或者没有释放_canJump
            if (!_jump || !_canJump)
                return;

            //如果没有着地
            if (!groundDetection.isOnGround)
                return;

            //if (_jumpButtonHeldDownTimer > _jumpToleranceTime)
            //    return;

            _canJump = false;           // Halt jump until jump button is released
            _isJumping = true;          // Update isJumping flag
            //_updateJumpTimer = true;    // Allow mid-air jump to be variable height

            FSM.SwitchState((int)PlayerState.Jump, null, null);
        }
        #endregion

        #region MonoBehaviour
        private void Awake()
        {
            FSM.Init(gameObject);
        }

        // Start is called before the first frame update
        IEnumerator Start()
        {
            _rigidbody =GetComponent<Rigidbody>();
            animator = _avatar._animator;
            movement = GetComponent<PlayerMovement>();

            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            FSM.SwitchState((int)PlayerState.GroundMovement, null, null);
        }

        // Update is called once per frame
        void Update()
        {
            //Update Input
            currentMoveDirection = new Vector3
            {
                x = Input.GetAxisRaw("Horizontal"),
                y = 0.0f,
                z = Input.GetAxisRaw("Vertical")
            };

            jump = Input.GetButton("Jump");
            CheckJump();

            fly = Input.GetKey(KeyCode.F);

            //Update FSM
            FSM.OnUpdate(Time.deltaTime, Time.realtimeSinceStartup);

            //Update Animator
            UpdateAnimator();
        }

        private void FixedUpdate()
        {
            FSM.OnFixedUpdate(Time.fixedDeltaTime, Time.realtimeSinceStartup);
        }

        private void LateUpdate()
        {
            FSM.OnLateUpdate(Time.deltaTime, Time.realtimeSinceStartup);
        }
        #endregion
    }
}