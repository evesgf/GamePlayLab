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
        public GameObject _avatar;
        internal Rigidbody _rigidbody;
        private Vector3 _currentMoveDireciton;
        private Vector3 _realMoveDirection;
        #endregion

        public Animator animator { get; set; }

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

        #region Methold
        private void UpdateAnimator()
        {
            var move = transform.InverseTransformDirection(realMoveDirection);
            animator.SetFloat("MoveSpeed", move.z, 0.1f, Time.deltaTime);
            animator.SetBool("OnGround", true);
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
            animator = _avatar.GetComponent<Animator>();

            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            FSM.SwitchState((int)PlayerState.GroundMovement, null, null);
        }

        // Update is called once per frame
        void Update()
        {
            currentMoveDirection = new Vector3
            {
                x = Input.GetAxisRaw("Horizontal"),
                y = 0.0f,
                z = Input.GetAxisRaw("Vertical")
            };

            FSM.OnUpdate(Time.deltaTime, Time.realtimeSinceStartup);

            FSM.txt_showState.text = ((PlayerState)FSM.GetCurStateID()).ToString();

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