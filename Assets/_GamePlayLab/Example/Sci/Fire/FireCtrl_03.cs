using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPL
{
    public class FireCtrl_03 : MonoBehaviour
    {
        public LayerMask targetLayer;

        public Bullet_03 bullet;

        public Transform Muzzle;

        private Bullet_03 bulletObj;
        private RaycastHit hit;

        // Start is called before the first frame update
        void Start()
        {

        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
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
            Destroy(bulletObj.gameObject);
        }
    }
}
