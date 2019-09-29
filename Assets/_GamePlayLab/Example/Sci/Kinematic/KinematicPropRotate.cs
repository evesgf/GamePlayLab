using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPL.KC
{
    [RequireComponent(typeof(Rigidbody))]
    public class KinematicPropRotate : MonoBehaviour
    {
        [Header("旋转")]
        [SerializeField]
        private float rotationSpeed=30f;

        private Rigidbody _rigidbody;

        private float angle;

        // Start is called before the first frame update
        void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.isKinematic = true;
        }

        private void FixedUpdate()
        {
            angle += rotationSpeed * Time.fixedDeltaTime;
            var r = Quaternion.Euler(0.0f, angle, 0.0f);
            _rigidbody.MoveRotation(r);
        }
    }
}
