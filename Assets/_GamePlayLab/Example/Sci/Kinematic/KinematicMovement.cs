using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPL.KC
{
    public class KinematicMovement : MonoBehaviour
    {
        [SerializeField]
        private bool _useGravity = true;
        [SerializeField]
        private float _gravity = 25.0f;

        private Rigidbody _rigidbody;
        private Vector3 _gravityDir=new Vector3(0,-1,0);

        private Vector3 currentVelocity;

        #region PROPERTIES
        public KinematicPlayer kinematicPlayer { get; set; }

        //重力开关
        public bool useGravity
        {
            get { return _useGravity; }
            set
            {
                _useGravity = value;

                // If gravity is disabled,
                // remove any remaining vertical velocity

                //if (!_useGravity)
                //    velocity = Vector3.ProjectOnPlane(velocity, Vector3.up);
            }
        }

        //重力
        public float gravity
        {
            get { return _gravity; }
            set { _gravity = Mathf.Max(0.0f, value); }
        }

        //角色的运动矢量
        public Vector3 velocity
        {
            get
            {
                if(_rigidbody==null) _rigidbody = GetComponent<Rigidbody>();
                return _rigidbody.velocity;
            }
            set { _rigidbody.velocity = value; }
        }

        //角色所受重力的方向
        public Vector3 gravityDir
        {
            get { return _gravityDir; }
            set { _gravityDir = value; }
        }
        #endregion

        #region METHOD
        
        /// <summary>
        /// 地面移动，恒定向下重力以便处理滑动
        /// </summary>
        /// <param name="desiredVelocity"></param>
        /// <param name="maxDesiredSpeed"></param>
        public void ApplyGroundMovement(Vector3 desiredVelocity, float maxDesiredSpeed, float acceleration,
            float deceleration)
        {
            var v= Vector3.ProjectOnPlane(velocity, -gravityDir);

            desiredVelocity = GetTangent(desiredVelocity,kinematicPlayer.groundDetection.surfaceNormal, -gravityDir);

            currentVelocity = desiredVelocity * maxDesiredSpeed;

            // Gravity
            if (useGravity) currentVelocity += gravityDir * gravity * Time.fixedDeltaTime;
        }

        /// <summary>
        /// 空中移动分开处理，用以控制空速和添加重力加速度
        /// </summary>
        /// <param name="desiredVelocity"></param>
        /// <param name="maxDesiredSpeed"></param>
        /// <param name="acceleration"></param>
        /// <param name="deceleration"></param>
        public void ApplyAirMovement(Vector3 desiredVelocity, float maxDesiredSpeed, float acceleration,
            float deceleration)
        {
            var v = Vector3.ProjectOnPlane(velocity, -gravityDir);

            desiredVelocity = GetTangent(desiredVelocity, kinematicPlayer.groundDetection.surfaceNormal, -gravityDir);

            currentVelocity = desiredVelocity * maxDesiredSpeed + gravityDir * gravity * Time.fixedDeltaTime;

            if (kinematicPlayer.groundDetection.isOnGround)
            {
                if (Vector3.Dot(currentVelocity, kinematicPlayer.groundDetection.surfaceNormal) <= 0.0f)
                {
                    var speedLimit = Mathf.Min(desiredVelocity.magnitude, maxDesiredSpeed);

                    var lateralVelocity = Vector3.ProjectOnPlane(velocity, -gravityDir);

                    desiredVelocity = Vector3.ProjectOnPlane(desiredVelocity, -gravityDir) +
                                      Vector3.Project(lateralVelocity, -gravityDir);

                    desiredVelocity = Vector3.ClampMagnitude(desiredVelocity, speedLimit);
                }
            }

            velocity = currentVelocity;

            // Gravity
            if (useGravity) velocity += gravityDir * gravity * Time.fixedDeltaTime;
        }

        /// <summary>
        /// 将移动方向映射到与向量相切的平面
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="normal"></param>
        /// <param name="up"></param>
        /// <returns></returns>
        public Vector3 GetTangent(Vector3 direction, Vector3 normal, Vector3 up)
        {
            var right = Vector3.Cross(direction, up);
            var tangent = Vector3.Cross(normal, right);

            return tangent.normalized;
        }

        /// <summary>
        /// 唯一更新角色运动速度的地方
        /// </summary>
        /// <param name="fixedDeltaTime"></param>
        public void UpdateVeleocity(float fixedDeltaTime)
        {
            //velocity = currentVelocity;
        }
        #endregion

        // Start is called before the first frame update
        void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void FixedUpdate()
        {
            kinematicPlayer.groundDetection.DetectGround();

            UpdateVeleocity(Time.fixedDeltaTime);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.gray;
            Gizmos.DrawLine(transform.position, transform.position+gravityDir * gravity);

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + velocity);
        }
    }
}
