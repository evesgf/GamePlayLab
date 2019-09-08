using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace GPL
{
    public class BaseGroundDetection : MonoBehaviour
    {
        #region EDITOR EXPOSED FIELDS
        [Tooltip("Layers to be considered as 'ground' (walkables).")]
        [SerializeField]
        private LayerMask _groundMask = 1;
        [Tooltip("Determines the maximum length of the cast.\n" +
         "As rule of thumb, configure it to your character's collider radius.")]
        [SerializeField]
        private float _castDistance = 0.5f;
        #endregion

        #region FIELDS
        protected const float kBackstepDistance = 0.05f;
        protected const float kMinCastDistance = 0.01f;
        public CapsuleCollider _capsuleCollider;
        protected GroundHit _groundHitInfo;
        #endregion

        #region PROPERTIES
        public LayerMask groundMask
        {
            get { return _groundMask; }
            set { _groundMask = value; }
        }
        /// <summary>
        /// Determines the max length of the cast.
        /// As rule of thumb, configure it to your character's collider radius.
        /// </summary>

        public float castDistance
        {
            get { return _castDistance; }
            set { _castDistance = Mathf.Max(kMinCastDistance, value); }
        }
        public CapsuleCollider capsuleCollider
        {
            get
            {
                if (_capsuleCollider == null) _capsuleCollider = GetComponent<CapsuleCollider>();
                return _capsuleCollider;
            }
        }

        public bool isOnGround
        {
            get { return _groundHitInfo.isOnGround; }
        }
        /// <summary>
        /// The contact point (in world space) where the cast hit the 'ground' collider.
        /// </summary>

        public Vector3 groundPoint
        {
            get { return _groundHitInfo.groundPoint; }
        }
        public Vector3 groundNormal
        {
            get { return _groundHitInfo.groundNormal; }
        }
        public Vector3 surfaceNormal
        {
            get { return _groundHitInfo.surfaceNormal; }
        }
        #endregion

        #region METHODS

        public void DetectGround()
        {
            _groundHitInfo = new GroundHit
            {
                groundPoint = transform.position,
                groundNormal = transform.up,
                surfaceNormal = transform.up
            };

            ComputeGroundHit(transform.position, transform.rotation, ref _groundHitInfo, castDistance);
        }

        public bool ComputeGroundHit(Vector3 position, Quaternion rotation, ref GroundHit groundHitInfo,
            float distance = Mathf.Infinity)
        {
            RaycastHit hitInfo;
            if (BottomSphereCast(position, rotation, out hitInfo, distance) &&
                Vector3.Angle(hitInfo.normal, rotation * Vector3.up) < 89.0f)
            {
                groundHitInfo.SetFrom(hitInfo);
                groundHitInfo.surfaceNormal = hitInfo.normal;
                groundHitInfo.isOnGround = true;

                return true;
            }
            else
            {
                return false;
            }
        }

        private bool BottomSphereCast(Vector3 position, Quaternion rotation, out RaycastHit hitInfo, float distance,
            float backstepDistance = kBackstepDistance)
        {
            var radius = capsuleCollider.radius;
            var height = Mathf.Max(0.0f, capsuleCollider.height * 0.5f - radius);
            var center = capsuleCollider.center - Vector3.up * height;

            var origin = position + rotation * center;
            var down = rotation * Vector3.down;

            return SphereCast(origin, radius, down, out hitInfo, distance, backstepDistance);
        }
        protected bool SphereCast(Vector3 origin, float radius, Vector3 direction, out RaycastHit hitInfo,
            float distance, float backstepDistance = kBackstepDistance)
        {
            origin = origin - direction * backstepDistance;

            var hit = Physics.SphereCast(origin, radius, direction, out hitInfo, distance + backstepDistance,
                groundMask, QueryTriggerInteraction.Ignore);
            if (hit)
                hitInfo.distance = hitInfo.distance - backstepDistance;

            return hit;
        }

        protected void DrawGizmos()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) return;

            if (!isOnGround) return;

            var color = new Color(0.0f, 1.0f, 0.0f, 0.25f);

            Handles.color = color;
            Handles.DrawSolidDisc(groundPoint, surfaceNormal, 0.1f);
#endif
        }
        #endregion

        #region MONOBEHAVIOUR
        private void OnDrawGizmosSelected()
        {
            DrawGizmos();
        }

        private void FixedUpdate()
        {

        }
        #endregion
    }
}
