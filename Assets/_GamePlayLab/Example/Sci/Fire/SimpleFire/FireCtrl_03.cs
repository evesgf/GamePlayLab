using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPL
{
    public class FireCtrl_03 : MonoBehaviour
    {
        public LayerMask targetLayer;
        public float angleLimit=10f;

        public Bullet_03 bullet;

        public Transform Muzzle;

        private NormalTurretCtrl normalTurretCtrl;
        private Bullet_03 bulletObj;
        private RaycastHit hit;

        // Start is called before the first frame update
        void Start()
        {
            normalTurretCtrl = GetComponent<NormalTurretCtrl>();
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if(normalTurretCtrl.GetAngleToTarget()<= angleLimit)
                    FireStart();
            }

            if (Input.GetMouseButtonUp(0))
            {
                FireEnd();
            }
        }

        public void FireStart()
        {
            bulletObj = Instantiate(bullet, Muzzle.position, Muzzle.rotation);
            bulletObj.transform.parent = Muzzle;
            bulletObj.Init();
        }

        public void FireEnd()
        {
            if(bulletObj!=null)
                Destroy(bulletObj.gameObject);
        }
    }
}
