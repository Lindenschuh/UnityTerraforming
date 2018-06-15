﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTerraforming.GameAi
{
    public class AttackPlayerAction : BehaviourNode
    {
        public override List<SteeringTypes> GetActions(BasicAi ai)
            => new List<SteeringTypes>()
            {
                SteeringTypes.ATTACK
            };
    }
}