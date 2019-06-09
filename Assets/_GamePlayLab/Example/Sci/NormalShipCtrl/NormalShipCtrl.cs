using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPL
{
    [RequireComponent(typeof(Rigidbody))]
    public class NormalShipCtrl : MonoBehaviour
    {
        public Transform avatar;
        public float maxForwardSpeed = 10f;
        public float forwoardAcc = 5f;
        public float forwardDamp = 5f;
        public float maxRotateSpeed = 10f;
        public float rotateDamp = 5f;
        public float maxUpSpeed = 10f;
        public float upAcc = 5f;
        public float upDamp = 5f;

        public float avatarUpAngle = 5f;
        public float avatarRotateAngle = 5f;
        public float avatarAngleStep = 5f;

        internal Vector3 moveDirection;
        internal float nowForwardSpeed;
        internal float nowRotateAngle;
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

            if (avatar != null)
            {
                avatar.localRotation = Quaternion.Lerp(avatar.localRotation, Quaternion.Euler(avatarUpAngle * -input.y, 0, avatarRotateAngle * -input.x), avatarAngleStep*Time.deltaTime);
            }
        }

        private void FixedUpdate()
        {
            OnMove(input); 
        }

        public void OnMove(Vector3 input)
        {
            //transform.Rotate(0, input.x * Time.fixedDeltaTime * rotateSpeed, 0);
            //transform.Translate(0, input.y * Time.fixedDeltaTime * upSpeed, input.z * Time.fixedDeltaTime * forwardSpeed);

            nowRotateAngle += input.x * Time.fixedDeltaTime * maxRotateSpeed;

            m_rigidbody.MoveRotation(Quaternion.Lerp(transform.rotation,Quaternion.Euler(0, nowRotateAngle, 0),rotateDamp*Time.fixedDeltaTime));

            if (input.z > float.Epsilon || input.z < -float.Epsilon)
            {
                nowForwardSpeed += forwoardAcc * Time.fixedDeltaTime*moveDirection.z;
            }
            else
            {
                nowForwardSpeed = Mathf.Lerp(nowForwardSpeed, 0,forwardDamp* Time.fixedDeltaTime);
            }
            nowForwardSpeed = Mathf.Clamp(nowForwardSpeed, -maxForwardSpeed, maxForwardSpeed);


            if (input.y > float.Epsilon || input.y < -float.Epsilon)
            {
                nowUpSpeed += upAcc * Time.fixedDeltaTime * input.y;
            }
            else
            {
                nowUpSpeed = Mathf.Lerp(nowUpSpeed, 0, upDamp*Time.fixedDeltaTime);
            }
            nowUpSpeed = Mathf.Clamp(nowUpSpeed,-maxUpSpeed,maxUpSpeed);

            moveDirection = transform.up * nowUpSpeed + transform.forward * nowForwardSpeed;
            m_rigidbody.MovePosition ( transform.position + moveDirection);
            Debug.DrawRay(transform.position, moveDirection);

            //m_rigidbody.velocity = new Vector3(0, input.y * Time.fixedDeltaTime * upSpeed, input.z * Time.fixedDeltaTime * forwardSpeed);

            //m_rigidbody.velocity = Quaternion.AngleAxis(input.x * Time.fixedDeltaTime * rotateSpeed, transform.up)*new Vector3(0, input.y * Time.fixedDeltaTime * upSpeed, input.z * Time.fixedDeltaTime * forwardSpeed);
        }
    }
}
