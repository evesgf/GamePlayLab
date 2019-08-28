using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPL
{
    public class PlayerTrail : MonoBehaviour
    {
        public Transform[] pos;
        public TrailRenderer[] trails;

        private bool isHigh = true;

        // Start is called before the first frame update
        void Start()
        {
            CloseTrail();
        }

        // Update is called once per frame
        void Update()
        {
            SetTrailPos();
        }

        private void SetTrailPos()
        {
            for (int i = 0; i < pos.Length; i++)
            {
                trails[i].transform.position = pos[i].position;
                trails[i].transform.rotation = pos[i].rotation;
            }
        }

        public void OpenTrail()
        {
            foreach (var t in trails)
            {
                t.enabled=true;
            }
        }

        public void CloseTrail()
        {
            foreach (var t in trails)
            {
                t.enabled = false;
            }
        }

        public void TrailHigh()
        {
            if (isHigh) return;
            foreach (var t in trails)
            {
                t.time = 0.5f;
            }
            isHigh = true;
        }

        public void TrailLow()
        {
            if (!isHigh) return;
            foreach (var t in trails)
            {
                t.time = 0.1f;
            }
            isHigh = false;
        }
    }
}
