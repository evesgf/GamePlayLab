using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPL.Movement
{
    public class MoveManager : MonoBehaviour
    {
        public CamRig camRig;

        public MovePlayerBase player_rig;
        public MovePlayerBase player_cc;

        private MovePlayerBase nowPlayer;

        // Start is called before the first frame update
        void Start()
        {
            SwtichPlayer(player_rig);
        }

        // Update is called once per frame
        void Update()
        {
            InputHandler();
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
