using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Invector.EventSystems
{
    public class vAnimatorStateListener : StateMachineBehaviour
    {
        public string[] tags = new string[] { "CustomAction", "LockMovement", "Attack" };
        public vAnimatorStateInfos stateInfos;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (stateInfos != null)
            {
                for (int i = 0; i < tags.Length; i++)
                    stateInfos.AddStateInfo(tags[i], stateInfo);
            }
            base.OnStateEnter(animator, stateInfo, layerIndex);
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (stateInfos != null)
            {
                for (int i = 0; i < tags.Length; i++)
                    stateInfos.UpdateStateInfo(tags[i], stateInfo);
            }
            base.OnStateUpdate(animator, stateInfo, layerIndex);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (stateInfos != null)
            {
                for (int i = 0; i < tags.Length; i++)
                    stateInfos.RemoveStateInfo(tags[i], stateInfo);
            }
            base.OnStateExit(animator, stateInfo, layerIndex);
        }
    }
}

