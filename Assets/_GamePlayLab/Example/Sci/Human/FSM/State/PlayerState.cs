﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPL
{
    public enum PlayerState
    {
        GroundIdle=0,
        GroundMovement=1,
        Jump=2,
        Fly=3,
        Fall=4,
        WallMovement=5,
        ClimbOverObs=6,
    }
}
