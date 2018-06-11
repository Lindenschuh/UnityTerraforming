using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTerraforming.GameAi
{
    public class AttackPlayerAction : Action
    {
        public override void MakeAction()
        {
            _stateManager.ChangeState(GuardianStates.ATTACK);
        }
    }
}