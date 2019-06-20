using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPL
{
    public class GunFireCtrl : MonoBehaviour
    {
        public LayerMask targetLayer;

        public Bullet_01 bullet;

        public float fireInterval = 0.1f;

        public Transform[] Muzzles;

        private float lastFireTime;
        private int muzzleFlag = 0;

        private RaycastHit hit;

        // Start is called before the first frame update
        void Start()
        {

        }

        private void Update()
        {
            //CheckFire
            //CheckTarget
            if (Physics.Raycast(Muzzles[muzzleFlag].forward, Muzzles[muzzleFlag].forward, out hit, 100f, targetLayer))
            {

            }
            else
            {

            }

            if (Input.GetMouseButton(0))
            {
                Fire();
            }
        }

        public void Fire()
        {
            if ((Time.time - lastFireTime) < fireInterval) return;

            var firePos = Muzzles[muzzleFlag];
            muzzleFlag = (muzzleFlag + 1) < Muzzles.Length ? muzzleFlag + 1 : 0;

            var b = Instantiate(bullet, firePos.position, firePos.rotation);
            b.GetComponent<Bullet_01>();
            b.Init();

            lastFireTime = Time.time;
        }
    }
}
