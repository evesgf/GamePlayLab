using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPL.KC
{
    public class KinematicTeleporter : MonoBehaviour
    {
        public KinematicTeleporter traget;
        public float nextTeleporterTime = 1f;

        GameObject TelObj;

        // Start is called before the first frame update
        void Start()
        {

        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag.Equals("Player") && TelObj==null)
            {
                OnTeleporter(other.gameObject);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            StartCoroutine(OnExit());
        }

        IEnumerator OnExit()
        {
            yield return new WaitForSeconds(nextTeleporterTime);
            TelObj = null;
        }

        public void OnTeleporter(GameObject gameObject)
        {
            traget.TelObj = gameObject;

            gameObject.transform.position = traget.transform.position;
            //gameObject.transform.rotation = traget.transform.rotation;

            print("OnTeleporter");
        }
    }
}

