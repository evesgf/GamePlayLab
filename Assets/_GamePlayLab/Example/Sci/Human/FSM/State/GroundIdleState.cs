using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPL
{
    public class GroundIdleState : StateBase
    {
        public override int GetStateID()
        {
            return (int)PlayerState.GroundIdle;
        }

        public override void OnFixedUpdate(float elapseSeconds, float realElapseSeconds)
        {

        }

        public override void OnLateUpdate(float elapseSeconds, float realElapseSeconds)
        {

        }

        public override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {

        }
    }
}
