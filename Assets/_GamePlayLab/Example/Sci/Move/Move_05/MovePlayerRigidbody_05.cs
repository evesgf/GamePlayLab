using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPL.Movement
{
    public class MovePlayerRigidbody_05 : MovePlayerBase
    {
        public override void FixedUpdate()
        {
            Movement.GroundCheck();

            //计算相机朝向即目标移动的正前方
            Vector3 _camlookDir = Vector3.ProjectOnPlane(cam.transform.forward, Vector3.up).normalized;
            Debug.DrawRay(transform.position, _camlookDir, Color.yellow);

            //通过叉乘得到左方向
            Vector3 left = Vector3.Cross(_camlookDir, Vector3.up);
            //法线方向和左方向叉乘得到指向前进的方向
            Vector3 newForward = Vector3.Cross(Vector3.up, left);
            //取得旋转
            Quaternion newRotation = Quaternion.LookRotation(newForward, Vector3.up);
            //沿旋转变换输入
            Vector3 _moveDir = newRotation * MoveDirection;
            Debug.DrawRay(transform.position, _moveDir, Color.red);

            //设置输入和旋转
            if (_moveDir != Vector3.zero)
            {
                Movement.Rotate(Quaternion.LookRotation(_moveDir), rotateSpeed, Time.fixedDeltaTime);
            }
            Movement.Move(_moveDir, moveSpeed, Time.fixedDeltaTime);
        }
    }
}
