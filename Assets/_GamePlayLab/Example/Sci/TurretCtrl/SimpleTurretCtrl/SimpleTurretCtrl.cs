using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GPL
{

    public class SimpleTurretCtrl : MonoBehaviour
    {
        public Transform realTargetIcon;
        public Text txtAngle;
        public bool debugShow = false;
        public bool useSlerp = true;

        public Transform swivel;            //旋转平台
        public float switelRotateSpeed;
        public Transform barrel;            //枪管
        public float barrelRotateSpeed;

        internal Vector3 target;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            Rotate();
            txtAngle.text = GetAngleToTarget().ToString("f2") ;
        }
        public float GetAngleToTarget()
        {
            return Vector3.Angle(barrel.forward, target - barrel.position);
        }


        private void Rotate()
        {
            //通过LookRotation找出枪管垂直旋转角度
            Vector3 targetVel = target - barrel.position;
            //指向目标的角度
            Quaternion targetRotationX = Quaternion.LookRotation(targetVel);
            //采用弧形差值或者速度朝向的方式转向目标角度
            barrel.rotation =useSlerp? Quaternion.Slerp(barrel.rotation, targetRotationX, switelRotateSpeed*Time.deltaTime):Quaternion.RotateTowards(barrel.rotation, targetRotationX, switelRotateSpeed * Time.deltaTime);
            //剔除其余角度旋转
            barrel.localEulerAngles = new Vector3(barrel.localEulerAngles.x, 0f, 0f);

            //通过LookRotation找出旋转平台的角度
            Vector3 targetY = target;
            targetY.y = barrel.position.y;
            //指向目标在枪管平面的投影向量
            Quaternion targetRotationY = Quaternion.LookRotation(targetY - swivel.position);

            swivel.rotation =useSlerp? Quaternion.Slerp(swivel.rotation, targetRotationY,barrelRotateSpeed*Time.deltaTime):Quaternion.RotateTowards(swivel.rotation, targetRotationY, barrelRotateSpeed * Time.deltaTime);
            //剔除其余角度旋转
            swivel.localEulerAngles = new Vector3(0f, swivel.localEulerAngles.y, 0f);

            //显示炮管真实指向
            var realPos = Camera.main.WorldToScreenPoint(barrel.position + barrel.forward * Vector3.Distance(barrel.position, target));
            realTargetIcon.position = new Vector3(realPos.x, realPos.y, 0);
        }

        private void OnDrawGizmos()
        {
            if (!debugShow) return;
            Gizmos.color = Color.red;

            Debug.DrawLine(barrel.position, barrel.position + barrel.forward * Vector3.Distance(barrel.position, target), Color.red);
        }
    }
}
