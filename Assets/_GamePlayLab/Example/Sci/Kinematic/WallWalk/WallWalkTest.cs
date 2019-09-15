using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace GPL
{
    public class WallWalkTest : MonoBehaviour
    {
        public Transform cam;

        public Vector3 moveDirection;

        public bool isWallWalking;
        public float walloffset = 0.6f;

        public float gravity = 9.8f;
        public Vector3 gravityDirection = Vector3.down;
        public Vector3 nowGravity;

        public float moveSpeed = 3f;
        public float rotateSpeed = 5f;

        public Vector3 velocity;

        private Rigidbody _rigidbody;

        private Vector3 lookInputVector;

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

            if (Input.GetKeyDown(KeyCode.F))
            {
                var origin = transform.position;
                origin.y += 1.4f;
                var dir = transform.forward;
                RaycastHit hit;
                if (Physics.Raycast(origin, dir, out hit, 5))
                {
                    //检测到墙面
                    Tweener t1 = transform.DOMove(hit.point - transform.forward * walloffset, 1f).OnComplete(() =>
                    {
                        isWallWalking = true;
                        ChangeGravityDir(-hit.normal);
                    });

                }
            }

            //RotationFromGravity();

            UseGravity();

            RotateTowardsMoveDirection(moveDirection);
            //Move(moveDirection);


            _rigidbody.velocity=velocity;
        }

        public void RotateTowardsMoveDirection(Vector3 moveDir)
        {
            var camPlanarDir = Vector3.ProjectOnPlane(cam.rotation * transform.forward, -gravityDirection).normalized;
            if (camPlanarDir.sqrMagnitude == 0f)
            {
                camPlanarDir = Vector3.ProjectOnPlane(cam.rotation * Vector3.up, -gravityDirection).normalized;
            }
            Quaternion cameraPlanarRotation = Quaternion.LookRotation(camPlanarDir, -gravityDirection);

            lookInputVector = cameraPlanarRotation * moveDir;
            Debug.DrawRay(transform.position, Vector3.ProjectOnPlane(transform.forward,-gravityDirection), Color.yellow);
            Debug.DrawRay(transform.position, lookInputVector, Color.red);

            Move(lookInputVector);
        }

        public void Move(Vector3 moveDir)
        {
            _rigidbody.MovePosition(transform.position+ moveDir*moveSpeed * Time.deltaTime);
            Debug.DrawRay(transform.position, transform.position + moveDir * moveSpeed * Time.deltaTime, Color.cyan);
        }

        public void RotationFromGravity()
        {
            transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.forward, -gravityDirection.normalized));
        }

        public void ChangeGravityDir(Vector3 dir)
        {

            gravityDirection = dir.normalized;

            transform.rotation = Quaternion.LookRotation(transform.rotation*dir);
            Debug.DrawRay(transform.position, Vector3.ProjectOnPlane(lookInputVector, -gravityDirection),Color.red);
        }

        public void UseGravity()
        {
            nowGravity = gravity * gravityDirection * Time.deltaTime;
            Debug.DrawRay(transform.position, gravity * gravityDirection, Color.black);
            velocity = nowGravity;
        }
    }
}
