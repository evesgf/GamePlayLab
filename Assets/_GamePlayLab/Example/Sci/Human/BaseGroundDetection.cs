using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPL
{
    public class BaseGroundDetection : MonoBehaviour
    {
        #region EDITOR EXPOSED FIELDS
        [Tooltip("Layers to be considered as 'ground' (walkables).")]
        [SerializeField]
        private LayerMask _groundMask = 1;

        #endregion

        #region FIELDS

        #endregion

        #region PROPERTIES
        /// <summary>
        /// Layers to be considered as 'ground' (walkables).
        /// </summary>

        public LayerMask groundMask
        {
            get { return _groundMask; }
            set { _groundMask = value; }
        }
        /// <summary>
        /// Is this character standing on ANY 'ground'?
        /// </summary>

        //public bool isOnGround
        //{
        //    get { return false; }
        //}
        #endregion

        #region METHODS
        /// <summary>
        /// Perform ground detection.
        /// </summary>
        public void DetectGround()
        {

        }
        #endregion

        #region MONOBEHAVIOUR

        #endregion
    }
}
