using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GPL
{
    public class PlanetWalkTest : MonoBehaviour
    {
        public Transform cam;

        public float gravity = 9.8f;

        private Vector3 _gravityDirection=Vector3.down;
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

        public Vector3 moveDirection;
        public Vector3 lookInputVector;
        public float moveSpeed=3f;
        public float rotateSpeed=5f;

        public bool isGrounded;

        private Rigidbody _rigidbody;

        // Start is called before the first frame update
        void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        // Update is called once per frame
        void Update()
        {
            moveDirection = new Vector3
            {
                x = Input.GetAxis("Horizontal"),
                y = 0f,
                z = Input.GetAxis("Vertical")
            };

            CheckGround();

            Rotate(moveDirection);
            Move(lookInputVector);

            //添加重力
            _rigidbody.AddForce(GravityDirection * gravity);
        }

        void Rotate(Vector3 moveDir)
        {
            var camPlanarDir = Vector3.ProjectOnPlane(cam.rotation * cam.forward, -GravityDirection).normalized;
            Debug.DrawRay(transform.position, camPlanarDir, Color.red);
            if (camPlanarDir.sqrMagnitude == 0f)
            {
                camPlanarDir = Vector3.ProjectOnPlane(cam.rotation * cam.up, -GravityDirection).normalized;
            }
            Quaternion cameraPlanarRotation = Quaternion.LookRotation(camPlanarDir, -GravityDirection);

            lookInputVector = cameraPlanarRotation * moveDir;
            if (lookInputVector != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(lookInputVector)*transform.rotation;
            }
            Debug.DrawRay(transform.position, lookInputVector, Color.yellow);
        }

        void Move(Vector3 moveDir)
        {
            moveDir = Vector3.ProjectOnPlane(moveDir, surfaceNormal);
            _rigidbody.MovePosition(transform.position + moveDir * moveSpeed * Time.deltaTime);
            Debug.DrawRay(transform.position, moveDir* moveSpeed, Color.cyan);
        }

        void CheckGround()
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, GravityDirection, out hit, 5f))
            {
                isGrounded = true;
                groundPoint = hit.point;
                surfaceNormal = hit.normal;
            }
            else
            {
                isGrounded = false;
            }
        }

        private void OnDrawGizmosSelected()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) return;

            var color = new Color(0.0f, 1.0f, 0.0f, 0.25f);

            Handles.color = color;
            Handles.DrawSolidDisc(groundPoint, surfaceNormal, 0.1f);
#endif
        }
    }
}
