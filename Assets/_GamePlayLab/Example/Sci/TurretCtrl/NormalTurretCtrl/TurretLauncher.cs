using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPL
{
    public class TurretLauncher : MonoBehaviour
    {
        public Transform targetIcon;
        public TurretBase[] turrets;

        private Vector3 target;

        // Start is called before the first frame update
        void Start()
        {
            target = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.farClipPlane));
            foreach (var turret in turrets)
            {
                turret.target = target;
            }
        }

        // Update is called once per frame
        void Update()
        {
            target = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.farClipPlane));
            foreach (var turret in turrets)
            {
                turret.target = target;
            }

            var pos = Camera.main.WorldToScreenPoint(target);
            targetIcon.position = new Vector3(pos.x, pos.y, 0);
        }
    }
}
