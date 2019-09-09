using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GPL.KC
{
    public class KinematicGroundDetectionInfo
    {
        public bool isOnGround { get; set; }

        public Vector3 groundPoint { get; set; }

        public Vector3 groundNormal { get; set; }

        public Vector3 surfaceNormal { get; set; }
    }

    public class KinematicGroundDetection : MonoBehaviour
    {
        public KinematicMovement movement;

        //地面层
        [SerializeField]
        private LayerMask _groundMask = 1;

        //地面检测投射距离
        [SerializeField]
        private float _castDistance = 0.5f;

        //角色胶囊体
        private CapsuleCollider _capsuleCollider;

        #region FIELDS
        protected const float kBackstepDistance = 0.05f;
        protected const float minCastDistance = 0.01f;          //最小检测距离

        private RaycastHit hitInfo;

        private float _referenceCastDistance;
        #endregion

        #region PROPERTIES
        public LayerMask groundMask
        {
            get { return _groundMask; }
            set { _groundMask = value; }
        }

        public CapsuleCollider capsuleCollider
        {
            get
            {
                if (_capsuleCollider == null) _capsuleCollider = GetComponent<CapsuleCollider>();
                return _capsuleCollider;
            }
        }

        //地面检测距离，一般为角色碰撞盒半径
        public float castDistance
        {
            get { return _castDistance; }
            set { _castDistance = Mathf.Max(minCastDistance, value); }
        }

        //地面检测信息
        public KinematicGroundDetectionInfo groundDetectionInfo { get; set; } = new KinematicGroundDetectionInfo();

        public bool isOnGround
        {
            get { return groundDetectionInfo.isOnGround; }
        }
        public Vector3 groundPoint
        {
            get { return groundDetectionInfo.groundPoint; }
        }
        public Vector3 groundNormal
        {
            get { return groundDetectionInfo.groundNormal; }
        }
        public Vector3 surfaceNormal
        {
            get { return groundDetectionInfo.surfaceNormal; }
        }
        #endregion

        #region METHOD
        //地面检测
        public void DetectGround()
        {
            capsuleCollider.isTrigger = true;

            ComputeGroundHit(transform.position, transform.rotation, castDistance);

            capsuleCollider.isTrigger = false;

            castDistance = isOnGround ? _referenceCastDistance : minCastDistance;
        }

        public bool ComputeGroundHit(Vector3 position, Quaternion rotation, 
            float distance = Mathf.Infinity)
        {
            if (BottomSphereCast(position, rotation, out hitInfo, distance) &&
                Vector3.Angle(hitInfo.normal, rotation * -movement.gravityDir) < 89.0f)
            {
                groundDetectionInfo.isOnGround = true;
                groundDetectionInfo.groundPoint = hitInfo.point;
                groundDetectionInfo.surfaceNormal = hitInfo.normal;

                return true;
            }
            else
            {
                groundDetectionInfo.isOnGround = false;

                return false;
            }
        }

        private bool BottomSphereCast(Vector3 position, Quaternion rotation, out RaycastHit hitInfo, float distance,
            float backstepDistance = kBackstepDistance)
        {
            var radius = capsuleCollider.radius;
            var height = Mathf.Max(0.0f, capsuleCollider.height * 0.5f - radius);
            var center = capsuleCollider.center - -movement.gravityDir * height;

            var origin = position + rotation * center;
            var down = rotation * movement.gravityDir;

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

            //if (!isOnGround) return;

            var color = new Color(0.0f, 1.0f, 0.0f, 0.25f);

            Handles.color = color;
            Handles.DrawSolidDisc(groundPoint, surfaceNormal, 0.1f);

            var radius = capsuleCollider.radius;
            var center = capsuleCollider.center;
            var offset = Mathf.Max(0.0f, capsuleCollider.height * 0.5f - radius);

            if (!Application.isPlaying)
                offset += castDistance;

            var c = new Color(0.5f, 1.0f, 0.6f);
            if (Application.isPlaying)
                c = isOnGround ? Color.blue : Color.red;

            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);

            Gizmos.color = c;
            Gizmos.DrawWireSphere(center - Vector3.up * offset, radius * 1.01f);

            Gizmos.matrix = Matrix4x4.identity;
#endif
        }
        #endregion

        // Start is called before the first frame update
        void Start()
        {
            _referenceCastDistance = castDistance;
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnDrawGizmosSelected()
        {
            DrawGizmos();
        }
    }
}
