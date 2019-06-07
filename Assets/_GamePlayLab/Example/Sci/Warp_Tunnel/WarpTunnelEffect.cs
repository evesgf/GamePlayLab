using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPL
{
    public class WarpTunnelEffect : MonoBehaviour
    {
        public float rotationSpeed;

        public float startDuration = 3f;
        public AnimationCurve colorCurve;
        public Color startColor;
        public Color durationColor;

        private Material _material;

        public void StartWarpTunnel(float startDuration)
        {
            StartWarpTunnel(startDuration, startColor, durationColor);
        }

        public void StartWarpTunnel(float startDuration, Color startColor, Color durationColor)
        {
            _material = gameObject.GetComponent<MeshRenderer>().material;
            StartCoroutine(OnStartWarpTunnel(startDuration, startColor, durationColor));
        }

        IEnumerator OnStartWarpTunnel(float startDuration, Color startColor, Color durationColor)
        {
            _material.SetColor("_node_6523", startColor);
            for (float i = 0; i < startDuration; i+=Time.deltaTime)
            {

                Color c = Color.Lerp(_material.GetColor("_node_6523"), durationColor, colorCurve.Evaluate(i / startDuration));
                _material.SetColor("_node_6523", c);
                yield return i;
            }
        }

        // Update is called once per frame
        void Update()
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, transform.rotation * Quaternion.Euler(rotationSpeed, 0, 0),
                Time.deltaTime);
        }
    }
}
