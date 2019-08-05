using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GPL
{
    public class GroundHit
    {
        #region FIELDS

        #endregion

        #region PROPERTIES
        public bool isOnGround { get; set; }

        public Vector3 groundPoint { get; set; }

        public Vector3 groundNormal { get; set; }

        public Vector3 surfaceNormal { get; set; }
        #endregion

        #region METHODS
        public void SetFrom(RaycastHit hitInfo)
        {
            groundPoint = hitInfo.point;
            groundNormal = hitInfo.normal;
        }
        #endregion
    }
}
