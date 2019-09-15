using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPL
{
    public class WallWalkPlanet : MonoBehaviour
    {
        public PlanetWalkTest walkTest;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            walkTest.GravityDirection = (transform.position - walkTest.transform.position);
        }
    }
}
