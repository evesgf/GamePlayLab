using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPL
{
    public class CharacterMovement : MonoBehaviour
    {
        #region EDITOR EXPOSED FIELDS

        #endregion

        #region FIELDS
        private Rigidbody _rigidbody;
        #endregion

        #region PROPERTIES
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
        #endregion

        #region METHODS

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
        #endregion
    }

}