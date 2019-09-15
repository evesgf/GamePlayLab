using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GPL
{
    public class WallWalkTest2 : MonoBehaviour
    {
        public Transform cam;

        public Vector3 moveDirection;

        public Vector3 velocity;

        private Rigidbody _rigidbody;
        private CapsuleCollider _capsuleCollider;

        //Gravity
        public bool isGrounded;
        public float gravity = 9.8f;
        private Vector3 _gravityDirection = Vector3.down;
        public Vector3 GravityDirection
        {
            get { return _gravityDirection; }
            set
            {
                _gravityDirection = value.normalized;
            }
        }
        private Vector3 groundPoint;
        private Vector3 surfaceNormal;

        //Wall
        public Transform wallCheckPoint;
        public bool isWallWalking;

        //Move
        public float moveSpeed = 3f;
        public float rotateSpeed = 5f;

        private Transform helper;

        // Start is called before the first frame update
        void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _capsuleCollider = GetComponent<CapsuleCollider>();

            helper = new GameObject().transform;
            helper.name = "helper";
        }

        // Update is called once per frame
        void Update()
        {
            InputHandler();

            if (isWallWalking)
            {
                GravityDirection = -surfaceNormal;
            }
            CheckGround();

            RotateToTarget();

            _rigidbody.velocity = GravityDirection * gravity;
        }

        //用户输入
        void InputHandler()
        {
            moveDirection = new Vector3
            {
                x = Input.GetAxis("Horizontal"),
                y = 0f,
                z = Input.GetAxis("Vertical")
            };

            if (Input.GetKeyDown(KeyCode.F))
            {
                OnTheWall();
            }

            if (Input.GetKeyDown(KeyCode.G))
            {
                isWallWalking = false;
                GravityDirection = Vector3.down;
            }
        }

        void Move(Vector3 moveDir)
        {
            _rigidbody.MovePosition(transform.position + moveDir * moveSpeed * Time.deltaTime);
        }

        void RotateToTarget()
        {
            Quaternion r1 = Quaternion.FromToRotation(transform.up, -GravityDirection) * transform.rotation;

            Vector3 lookInputVector = GetTangent(moveDirection, surfaceNormal, -GravityDirection);
            Debug.DrawRay(transform.position, lookInputVector, Color.yellow);

            Quaternion r = r1;

            Vector3 v = moveDirection;
            Vector3 x = r * v;

            Debug.DrawRay(transform.position, x, Color.blue);

            helper.position = transform.position;
            helper.rotation = r1;

            transform.rotation = Quaternion.Slerp(transform.rotation, r1, rotateSpeed * Time.deltaTime);

            Move(x);
        }

        public Vector3 GetTangent(Vector3 direction, Vector3 normal, Vector3 up)
        {
            var right = Vector3.Cross(direction, up);
            var tangent = Vector3.Cross(normal, right);

            return tangent.normalized;
        }

        void OnTheWall()
        {
            var origin = transform.position;
            origin.y += 1.4f;
            var dir = transform.forward;
            RaycastHit hit;
            if (Physics.Raycast(origin, dir, out hit, 5))
            {
                //检测到墙面
                Tweener t1 = transform.DOMove(hit.point - transform.forward * _capsuleCollider.height*0.5f, 1f).OnComplete(() =>
                {
                    isWallWalking = true;
                    Debug.DrawLine(origin, hit.point, Color.red, 1f);
                });

                //计算旋转角度
                Quaternion r1 = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;

                Tweener t2 = transform.DORotateQuaternion(r1,1f);
            }
        }

        //地面检测
        void CheckGround()
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, -transform.up, out hit, 5f))
            {
                isGrounded = true;
                groundPoint = hit.point;
                surfaceNormal = hit.normal;
                Debug.DrawLine(transform.position, hit.point, Color.red);
            }
            else
            {
                isGrounded = false;
                Debug.DrawRay(transform.position, -transform.up * 5f, Color.red);
            }
        }

        private void OnDrawGizmos()
        {
            DrawGizmos();
        }

        protected void DrawGizmos()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) return;

            //if (!isOnGround) return;

            var color = new Color(0.0f, 1.0f, 0.0f, 0.25f);

            Handles.color = color;
            Handles.DrawSolidDisc(groundPoint, surfaceNormal, 0.1f);

            Debug.DrawRay(transform.position, GravityDirection * gravity, Color.black);
#endif
        }
    }
}
