using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPL
{

    [RequireComponent(typeof(Rigidbody))]
    public class SimpleShipCtrl : MonoBehaviour
    {
        public Transform avatar;
        public float forwardSpeed = 10f;
        public float rotateSpeed = 10f;
        public float upSpeed = 10f;

        public float avatarUpAngle = 5f;
        public float avatarRotateAngle = 5f;
        public float avatarAngleStep = 5f;

        internal Vector3 moveDirection;
        internal float nowForwardSpeed;
        internal float nowRotateSpeed;
        internal float nowUpSpeed;

        internal Vector3 input;

        private Rigidbody m_rigidbody;

        // Start is called before the first frame update
        void Start()
        {
            m_rigidbody = GetComponent<Rigidbody>();
        }

        // Update is called once per frame
        void Update()
        {
            input.Set(Input.GetAxis("Horizontal"), Input.GetAxis("UpDown"), Input.GetAxis("Vertical"));
        }

        private void FixedUpdate()
        {
            OnMove(input);
        }

        public void OnMove(Vector3 input)
        {
            transform.Rotate(0, input.x * Time.fixedDeltaTime * rotateSpeed, 0);
            transform.Translate(0, input.y * Time.fixedDeltaTime * upSpeed, input.z * Time.fixedDeltaTime * forwardSpeed);

            if (avatar != null)
            {
                avatar.localRotation = Quaternion.Lerp(avatar.localRotation, Quaternion.Euler(avatarUpAngle * -input.y, 0, avatarRotateAngle * -input.x), avatarAngleStep*Time.fixedDeltaTime);
            }
        }
    }
}
