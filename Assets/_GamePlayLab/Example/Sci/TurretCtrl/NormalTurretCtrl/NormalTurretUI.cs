using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GPL
{
    public class NormalTurretUI : MonoBehaviour
    {
        public Transform realTargetIcon;
        public Text txtAngle;

        private NormalTurretCtrl normalleTurretCtrl;

        // Start is called before the first frame update
        void Start()
        {
            normalleTurretCtrl = GetComponent<NormalTurretCtrl>();
        }

        // Update is called once per frame
        void Update()
        {
            txtAngle.text = normalleTurretCtrl.GetAngleToTarget().ToString("f2");

            //显示炮管真实指向
            var screenPos = normalleTurretCtrl.GetTargetDistance();
            realTargetIcon.position = new Vector3(screenPos.x, screenPos.y, 0);
        }
    }
}

