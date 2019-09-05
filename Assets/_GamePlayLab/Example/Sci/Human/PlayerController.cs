using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPL
{
    public class PlayerController : MonoBehaviour
    {
        public BaseGroundDetection groundDetection;
        public WallDetection wallDetection;

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

        private bool _isFall;

        private bool _sprint;
        private bool _isSprinting;
        private bool _isSprintStop;

        private bool _wall;

        private bool _isClimbOverObs;
        #endregion

        #region PROPERTIES
        public CapsuleCollider capsuleCollider { get; set; }

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

        /// <summary>
        /// 用于锁定Horizontal方向的Animaotr输入
        /// </summary>
        public bool LockHorizontal { get; set; }

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
            get { return _isJumping; }
            set { _isJumping = value; }
        }

        public bool fly
        {
            get { return _fly; }
            set
            {
                if (value != true) return;
                if (!_fly)
                {
                    if (!groundDetection.isOnGround)
                    {
                        FSM.SwitchState((int)PlayerState.Fly, null, null);
                        _fly = true;
                    } 
                }else
                {
                    FSM.SwitchState((int)PlayerState.GroundMovement, null, null);
                    _fly = false;
                }
            }
        }

        public bool fall
        {
            get { return _isFall; }
            set { _isFall = value; }
        }

        public bool sprint
        {
            get { return _sprint; }
            set
            {
                if (value != true) return;

                _sprint = !_sprint;
                isSprinting = _sprint;
            }
        }
        public bool isSprinting
        {
            get { return _isSprinting; }
            set
            {
                _isSprinting = value;
                _sprint = value;
                if (value == true)
                {
                    _isSprintStop = false;
                }
            }
        }

        public bool isSprintStop
        {
            get { return _isSprintStop; }
            set { _isSprintStop = value; }
        }

        public bool isWall
        {
            get { return _wall; }
            set { _wall = value; }
        }

        public bool isClimbOverObs
        {
            get { return _isClimbOverObs; }
            set { _isClimbOverObs=value; }
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

            _avatar.SetFloat("Horizontal", LockHorizontal ? 0 : currentMoveDirection.x, 0.1f, Time.deltaTime);
            _avatar.SetFloat("Vertical",currentMoveDirection.z, 0.1f, Time.deltaTime);
            _avatar.SetFloat("UpAxisDirection", currentMoveDirection.y, 0.1f, Time.deltaTime);

            _avatar.SetBool("IsFly", _fly);
            _avatar.SetBool("IsJumping", _isJumping);
            _avatar.SetBool("IsFall", _isFall);

            _avatar.SetBool("IsSprinting", _isSprinting);
            _avatar.SetBool("IsSprintStop", _isSprintStop);

            _avatar.SetBool("IsWall", _wall);

            _avatar.SetBool("IsClimbOverObs", _isClimbOverObs);
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

            _canJump = false;           // Halt jump until jump button is released

            FSM.SwitchState((int)PlayerState.Jump, null, null);
        }

        /// <summary>
        /// Fall状态检查
        /// </summary>
        public void CheckFall()
        {
            if (!fly && !isJumping && !groundDetection.isOnGround && !isWall && !isClimbOverObs)
            {
                FSM.SwitchState((int)PlayerState.Fall, null, null);
            }
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
            capsuleCollider = GetComponent<CapsuleCollider>();

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
                y = Input.GetAxisRaw("UpAxis"),
            z = Input.GetAxisRaw("Vertical")
            };

            jump = Input.GetButton("Jump");
            CheckJump();

            fly = Input.GetKeyDown(KeyCode.F);

            sprint= Input.GetKeyDown(KeyCode.LeftShift);

            CheckFall();

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