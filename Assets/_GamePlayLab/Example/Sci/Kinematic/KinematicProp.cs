using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPL.KC
{
    [RequireComponent(typeof(Rigidbody))]
    public class KinematicProp : MonoBehaviour
    {
        [Header("移动")]
        public bool useMove;
        [SerializeField]
        public float moveDuration = 3.0f;
        [SerializeField]
        private Transform target;
        [SerializeField]
        private Ease moveEase = Ease.Linear;

        [Header("旋转")]
        public bool useRotation;
        [SerializeField]
        private Vector3 rotationSpeed;
        [SerializeField]
        public float rotationDuration = 3.0f;
        [SerializeField]
        public Quaternion targetAngle;
        [SerializeField]
        private Ease rotationEase = Ease.Linear;
        [Header("Loop/PingPong")]
        [SerializeField]
        private bool rotationIsPingPong;

        private Rigidbody _rigidbody;

        private Vector3 startPosition;
        private Quaternion startAngle;

        // Start is called before the first frame update
        void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.isKinematic = true;

            startPosition = transform.position;
            startAngle = transform.rotation;

            if (useMove) DoMove();
            if (useRotation) DoRotation();
        }

        void DoMove()
        {
            Tweener tMove = transform.DOMove(target.position, moveDuration).SetEase(moveEase).OnComplete(() => {
                Tween t = transform.DOMove(startPosition, moveDuration).SetEase(moveEase).OnComplete(() => {
                    DoMove();
                });
            });
        }

        void DoRotation()
        {
            if (rotationIsPingPong)
            {
                Tweener tRotation = transform.DORotateQuaternion(targetAngle, rotationDuration).SetEase(rotationEase).OnComplete(() => {
                    Tweener t = transform.DORotateQuaternion(startAngle, rotationDuration).SetEase(rotationEase).OnComplete(() => {
                        DoRotation();
                    });
                });
            }
        }

        private void FixedUpdate()
        {
            if (useRotation && !rotationIsPingPong)
            {
                _rigidbody.DORotate(rotationSpeed*Time.time, Time.fixedDeltaTime);
            }
        }
    }
}
