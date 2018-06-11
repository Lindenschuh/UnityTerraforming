using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTerraforming.GameAi
{
    public class SeekGuardianRadiusAction : Action
    {
        public override void MakeAction()
        {
            _stateManager.ChangeState(GuardianStates.OUTSIDE);
        }
    }
}