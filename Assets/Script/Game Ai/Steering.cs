﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTerraforming.GameAi
{
    public class Steering
    {
        public float angualr;
        public Vector3 linear;

        public Steering()
        {
            angualr = 0;
            linear = new Vector3();
        }
    }
}