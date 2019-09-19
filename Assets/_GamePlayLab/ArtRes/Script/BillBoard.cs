using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPL
{
    public class BillBoard : MonoBehaviour
    {
        public Transform cam;

        // Start is called before the first frame update
        void Start()
        {
            if (cam == null) cam = Camera.main.transform;
        }

        // Update is called once per frame
        void Update()
        {
            transform.forward = cam.forward;
            transform.rotation = cam.rotation;
        }
    }
}
