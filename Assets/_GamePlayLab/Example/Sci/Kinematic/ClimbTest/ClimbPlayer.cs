using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPL
{
    public class ClimbPlayer : MonoBehaviour
    {
        public bool isClimbing;

        bool inPosition;
        bool isLerping;
        float t;

        Vector3 startPos;
        Vector3 targetPos;
        Quaternion startRot;
        Quaternion targetRot;
        public float positionOffset=0.4f;
        public float offsetFromWall = 0.3f;
        public float speed_multiplier = 0.2f;
        public float climbSpeed = 3f;
        public float rotateSpeed = 5f;
        public float inAngleDis = 1f;

        public float horizontal;
        public float vertical;

        private Transform helper;
        float delta;
        private Vector3 moveDir;

        // Start is called before the first frame update
        void Start()
        {
            Init();
        }

        public void Init()
        {
            helper = new GameObject().transform;
            helper.name = "Climb Helper";

            CheckForClimb();
        }

        // Update is called once per frame
        void Update()
        {
            delta += Time.deltaTime;
            Tick(delta);
        }

        public void CheckForClimb()
        {
            var origin = transform.position;
            origin.y += 1.4f;
            var dir = transform.forward;
            RaycastHit hit;
            if (Physics.Raycast(origin, dir, out hit, 5))
            {
                helper.position = PosWithOffset(origin, hit.point);
                InitForClimb(hit);
            }
        }

        void InitForClimb(RaycastHit hit)
        {
            isClimbing = true;
            helper.transform.rotation = Quaternion.LookRotation(-hit.normal);
            startPos = transform.position;
            targetPos = hit.point + (hit.normal * offsetFromWall);
            t = 0;
            inPosition = false;
        }

        public void Tick(float delta)
        {
            if (!inPosition)
            {
                GetInPosition(delta);
                return;
            }

            if (!isLerping)
            {
                horizontal = Input.GetAxis("Horizontal");
                vertical = Input.GetAxis("Vertical");
                float m = Mathf.Abs(horizontal) + Mathf.Abs(vertical);

                Vector3 h = helper.right * horizontal;
                Vector3 v = helper.up * vertical;
                moveDir = (h + v).normalized;
                bool canMove=CanMove(moveDir);

                if (!canMove || moveDir == Vector3.zero) return;

                t = 0;
                isLerping = true;
                startPos = transform.position;
                //Vector3 tp = helper.position - transform.position;

                targetPos = helper.position;
            }
            else
            {
                t += delta * climbSpeed;
                if (t > 1)
                {
                    t = 1;
                    isLerping = false;
                }
                Vector3 cp = Vector3.Lerp(startPos, targetPos, t);
                transform.position=cp;
                transform.rotation = Quaternion.Slerp(transform.rotation, helper.rotation, delta * rotateSpeed);
            }
        }

        bool CanMove(Vector3 moveDir)
        {
            Vector3 origin = transform.position;
            float dis = positionOffset;
            Vector3 dir = moveDir;
            Debug.DrawRay(origin, dir * dis,Color.red);
            RaycastHit hit;
            if (Physics.Raycast(origin, dir, out hit, dis))
            {
                return false;
            }

            origin += moveDir * dis;
            dir = helper.forward;
            float dis2 = inAngleDis;
            Debug.DrawRay(origin, dir * dis2,Color.blue);
            if (Physics.Raycast(origin, dir, out hit, dis))
            {
                helper.position = PosWithOffset(origin, hit.point);
                helper.rotation = Quaternion.LookRotation(-hit.normal);
                return true;
            }

            origin += dir * dis2;
            dir = -Vector3.up;
            Debug.DrawRay(origin, dir,Color.green);
            if (Physics.Raycast(origin, dir, out hit, dis2))
            {
                float angle = Vector3.Angle(helper.up,hit.normal);
                if (angle < 40)
                {
                    helper.position = PosWithOffset(origin, hit.point);
                    helper.rotation = Quaternion.LookRotation(-hit.normal);
                    return true;
                }
            }

            return false;
        }

        void GetInPosition(float delta)
        {
            t += delta;

            if (t > 1)
            {
                t = 1;
                inPosition = true;
            }

            Vector3 tp = Vector3.Lerp(startPos, targetPos, t);
            transform.position = tp;
            transform.rotation = Quaternion.Slerp(transform.rotation, helper.rotation, delta * rotateSpeed);
        }

        Vector3 PosWithOffset(Vector3 origin, Vector3 target)
        {
            Vector3 direction = origin - target;
            direction.Normalize();
            Vector3 offset = direction * offsetFromWall;
            return target + offset;
        }
    }
}
