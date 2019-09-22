using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GPL.Movement
{
    public class MoveManager : MonoBehaviour
    {
        public CamRig camRig;

        [Header("Player")]
        public MovePlayerBase player_rig;
        public MovePlayerBase player_cc;

        private MovePlayerBase nowPlayer;

        [Header("PropertyUI")]
        public Text txt_moveSpeed;

        // Start is called before the first frame update
        void Start()
        {
            SwtichPlayer(player_rig);
        }

        // Update is called once per frame
        void Update()
        {
            InputHandler();

            ShowProperty();
        }

        public void ShowProperty()
        {
            if (txt_moveSpeed != null) txt_moveSpeed.text = nowPlayer.NowMoveSpeed.ToString("f2");
        }

        public void SwtichMoveType()
        {
            //nowPlayer.SwtichMoveType(moveType);
        }

        public void SwtichPlayer(MovePlayerBase player)
        {
            if (player != nowPlayer)
            {
                player_rig.gameObject.SetActive(false);
                player_cc.gameObject.SetActive(false);

                player.gameObject.SetActive(true);
                nowPlayer = player;
            }

            camRig.target = nowPlayer.camTarget;
        }

        public void InputHandler()
        {
            nowPlayer.MoveDirection = new Vector3
            {
                x = Input.GetAxis("Horizontal"),
                y = 0f,
                z = Input.GetAxis("Vertical")
            };
        }
    }
}
