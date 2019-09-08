using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GPL.KC
{
    public class KinematicDebugUI : MonoBehaviour
    {
        public Text txt_Input;
        public Text txt_FSM;
        public Text txt_IsGround;
        public Text txt_Velocity;

        public KinematicPlayer playerController;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            txt_Input.text = playerController.currentMoveDirection.normalized.ToString();
            txt_IsGround.text = playerController.groundDetection.isOnGround.ToString();
            txt_Velocity.text = playerController.movement.velocity.ToString();
        }
    }
}
