using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPL
{
    public class WarpTunnelEngineCtrl : MonoBehaviour
    {
        public Transform target;
        public WarpTunnelEngine warpTunnelEngine;

        // Start is called before the first frame update
        void Start()
        {
            if (warpTunnelEngine == null)
                warpTunnelEngine = GetComponent<WarpTunnelEngine>();
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.J))
            {
                warpTunnelEngine.WarpTunnelStart(target.position);
            }
        }
    }
}