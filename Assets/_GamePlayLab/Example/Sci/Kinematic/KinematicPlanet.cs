using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPL.KC
{
    public class KinematicPlanet : MonoBehaviour
    {
        public Transform gravityCenter;

        private List<KinematicMovement> AffectedObjs;

        // Start is called before the first frame update
        void Start()
        {
            AffectedObjs = new List<KinematicMovement>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag.Equals("Player"))
            {
                AffectedObjs.Add(other.GetComponent<KinematicMovement>());
            }
        }

        private void OnTriggerStay(Collider other)
        {
            foreach (var obj in AffectedObjs)
            {
                obj.gravityDir = (gravityCenter.position- obj.transform.position).normalized;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.tag.Equals("Player") && AffectedObjs.Contains(other.GetComponent<KinematicMovement>()))
            {
                other.GetComponent<KinematicMovement>().gravityDir = Vector3.down;
                AffectedObjs.Remove(other.GetComponent<KinematicMovement>());
            }
        }
    }
}
