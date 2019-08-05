using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPL
{
    public class CharacterMovement : MonoBehaviour
    {
        #region EDITOR EXPOSED FIELDS
        [Tooltip("Enable / disable character's custom gravity." +
         "If enabled the character will be affected by this gravity force.")]
        [SerializeField]
        private bool _useGravity = true;
        [Tooltip("The gravity applied to this character.")]
        [SerializeField]
        private float _gravity = 25.0f;
        [Tooltip("The amount of gravity to be applied when sliding off a steep slope.")]
        [SerializeField]
        private float _slideGravityMultiplier = 2.0f;
        #endregion

        #region FIELDS
        private Rigidbody _rigidbody;
        #endregion

        #region PROPERTIES
        /// <summary>
        /// 角色的速度
        /// </summary>
        public Vector3 velocity
        {
            get { return _rigidbody.velocity - platformVelocity; }
            set { _rigidbody.velocity = value + platformVelocity; }
        }

        /// <summary>
        /// The velocity of the platform the character is standing on,
        /// zero (Vector3.zero) if not on a platform.
        /// </summary>
        public Vector3 platformVelocity { get; private set; }

        private BaseGroundDetection groundDetection { get; set; }
        /// <summary>
        /// Enable / disable character's gravity.
        /// If enabled the character will be affected by its custom gravity force.
        /// </summary>

        public bool useGravity
        {
            get { return _useGravity; }
            set
            {
                _useGravity = value;

                // If gravity is disabled,
                // remove any remaining vertical velocity

                if (!_useGravity)
                    velocity = Vector3.ProjectOnPlane(velocity, Vector3.up);
            }
        }
        /// <summary>
        /// The amount of gravity to be applied to this character.
        /// We apply gravity manually for more tuning control.
        /// </summary>

        public float gravity
        {
            get { return _gravity; }
            set { _gravity = Mathf.Max(0.0f, value); }
        }
        /// <summary>
        /// The percentage of gravity that will be applied to the slide.
        /// </summary>

        public float slideGravityMultiplier
        {
            get { return _slideGravityMultiplier; }
            set { _slideGravityMultiplier = Mathf.Max(1.0f, value); }
        }
        /// <summary>
        /// The amount of gravity to apply when sliding.
        /// </summary>

        public float slideGravity
        {
            get { return gravity * slideGravityMultiplier; }
        }
        #endregion

        #region METHODS
        /// <summary>
        /// Perform ground detection.
        /// </summary>
        private void DetectGround()
        {
            groundDetection.DetectGround();
        }

        /// <summary>
        /// 设置角色移动
        /// </summary>
        /// <param name="desiredVelocity"></param>
        /// <param name="acceleration"></param>
        public void Move(Vector3 desiredVelocity, float maxDesiredSpeed, float acceleration,float deceleration)
        {
            DetectGround();

            ApplyGroundMovement(desiredVelocity, maxDesiredSpeed, acceleration,deceleration);
        }

        /// <summary>
        /// Returns the direction adjusted to be tangent to a specified surface normal relatively to given up axis.
        /// </summary>
        /// <param name="direction">The direction to be adjusted.</param>
        /// <param name="normal">The surface normal.</param>
        /// <param name="up">The given up-axis.</param>
        public static Vector3 GetTangent(Vector3 direction, Vector3 normal, Vector3 up)
        {
            var right = Vector3.Cross(direction, up);
            var tangent = Vector3.Cross(normal, right);

            return tangent.normalized;
        }

        /// <summary>
        /// 设置在地面上的加速度和摩擦力运动
        /// </summary>
        private void ApplyGroundMovement(Vector3 desiredVelocity, float maxDesiredSpeed, float acceleration,float deceleration)
        {
            var desiredSpeed = desiredVelocity.magnitude;

            var desiredDirection = GetTangent(desiredVelocity,transform.up,Vector3.up);

            var desiredAcceleration = desiredDirection * acceleration * Time.fixedDeltaTime;
            if (desiredAcceleration.sqrMagnitude < float.Epsilon)
            {
                velocity = Vector3.MoveTowards(velocity, desiredVelocity, deceleration * Time.deltaTime);
            }
            else
            {
                velocity =Vector3.ClampMagnitude(velocity+desiredAcceleration, maxDesiredSpeed);
            }

            if (useGravity)
            {
                desiredVelocity = Vector3.MoveTowards(velocity, desiredVelocity, Mathf.Min(acceleration, gravity) * Time.deltaTime);
                velocity += Vector3.ProjectOnPlane(desiredVelocity - velocity, Vector3.up) +
                            Vector3.down * (slideGravity * Time.deltaTime);
            }
        }
        #endregion

        #region MONOBEHAVIOUR
        private void Awake()
        {
            groundDetection = GetComponent<BaseGroundDetection>();
            if (groundDetection == null)
            {

            }

            _rigidbody = GetComponent<Rigidbody>();
            if (_rigidbody == null)
            {

            }
        }

        private void FixedUpdate()
        {
            //var p = transform.position;
            //var r = transform.rotation;



            //_rigidbody.MovePosition(p);
            //_rigidbody.MoveRotation(r);
        }
        #endregion
    }

}