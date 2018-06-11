using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTerraforming.GameAi
{
    public class WanderAction : Action
    {
        public override void MakeAction()
        {
            _stateManager.ChangeState(GuardianStates.WANDER);
        }
    }
}