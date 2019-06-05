using UnityEngine;
using System.Collections;
using System.Reflection;

namespace Slate
{

    [System.Serializable]
    public struct TransformationParameter : ITransformableHelperParameter
    {

        [SerializeField]
        private Transform _transform;
        [SerializeField]
        private Vector3 _position;
        [SerializeField]
        private Vector3 _rotation;
        [SerializeField]
        private TransformSpace _space;

        public bool useAnimation {
            get { return _transform == null; }
        }

        public TransformSpace space {
            get { return transform != null ? TransformSpace.WorldSpace : _space; }
            private set { _space = value; }
        }

        public Transform transform {
            get { return _transform; }
            private set { _transform = value; }
        }

        public Vector3 position {
            get { return transform != null ? transform.position : _position; }
            set { _position = value; }
        }

        public Vector3 rotation {
            get { return transform != null ? transform.eulerAngles : _rotation; }
            set { _rotation = value; }
        }

        public override string ToString() {
            return _transform != null ? _transform.name : _position.ToString();
        }
    }
}