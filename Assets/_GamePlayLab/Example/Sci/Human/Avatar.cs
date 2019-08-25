using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPL
{
    public class Avatar : MonoBehaviour
    {
        public GameObject _avatarObj;
        public Animator _animator;

        public bool usePants = true;
        public Animator pants;

        #region METHODS
        public void SetFloat(string name, float value, float dampTime=0, float deltaTime=0)
        {
            _animator.SetFloat(name, value, dampTime, deltaTime);

            if (pants != null && usePants)
            {
                pants.SetFloat(name, value, dampTime, deltaTime);
            }
        }

        public void SetBool(string name, bool value)
        {
            _animator.SetBool(name, value);

            if (pants != null && usePants)
            {
                pants.SetBool(name, value);
            }
        }

        public void SetInteger(string name, int value)
        {
            _animator.SetInteger(name, value);

            if (pants != null && usePants)
            {
                pants.SetInteger(name, value);
            }
        }

        public void SetTrigger(string name)
        {
            _animator.SetTrigger(name);

            if (pants != null && usePants)
            {
                pants.SetTrigger(name);
            }
        }
        #endregion

        private void Start()
        {
            if (pants != null)
            {
                pants.avatar = _animator.avatar;
            }
        }
    }
}
