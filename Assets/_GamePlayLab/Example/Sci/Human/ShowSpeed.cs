using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPL
{
    public class ShowSpeed : MonoBehaviour
    {
        private Rigidbody _rigidbody;
        private BaseCharacterController characterController;

    // Start is called before the first frame update
    void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            characterController = GetComponent<BaseCharacterController>();
        }

        private void OnGUI()
        {
            GUI.Label(new Rect(100, 100, 200, 80), characterController.moveDirection.magnitude.ToString("f2"));
        }
    }
}
