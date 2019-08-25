using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPL
{
    public class AvatarLeftFoot : MonoBehaviour
    {
        private Avatar avatar;

        // Start is called before the first frame update
        void Start()
        {
            avatar = GetComponentInParent<Avatar>();
        }

        public void IsLeftFoot(float value)
        {
            avatar.SetFloat("LeftFoot", value);
        }
    }
}
