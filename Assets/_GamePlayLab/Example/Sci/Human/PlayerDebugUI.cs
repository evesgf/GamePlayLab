using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GPL
{
    public class PlayerDebugUI : MonoBehaviour
    {
        public Text txt_Input;
        public Text txt_FSM;
        public Text txt_IsGround;
        public Text txt_Velocity;

        public PlayerController playerController;

    // Start is called before the first frame update
    void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            txt_Input.text = playerController.currentMoveDirection.normalized.ToString();
            txt_FSM.text = ((PlayerState)playerController.FSM.GetCurStateID()).ToString();
            txt_IsGround.text = playerController.groundDetection.isOnGround.ToString();
            txt_Velocity.text = playerController.movement.velocity.ToString("f2");
        }
    }
}
