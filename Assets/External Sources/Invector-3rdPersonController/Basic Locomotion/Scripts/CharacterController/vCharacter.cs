using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;
namespace Invector.vCharacterController
{
    using EventSystems;
    [System.Serializable]
    public class OnActiveRagdoll : UnityEvent { }
    [System.Serializable]
    public class OnActionHandle : UnityEvent<Collider> { }
    [System.Serializable]

    [vClassHeader("vCharacter")]
    public abstract class vCharacter : vHealthController
    {
        #region Character Variables 

        public enum DeathBy
        {
            Animation,
            AnimationWithRagdoll,
            Ragdoll
        }

        [vEditorToolbar("Health")]
        public DeathBy deathBy = DeathBy.Animation;
        public bool removeComponentsAfterDie;
        // get the animator component of character
        [HideInInspector]
        public Animator animator { get; private set; }
        // know if the character is ragdolled or not
        // [HideInInspector]
        public bool ragdolled { get; set; }

        [vEditorToolbar("Events")]
        [Header("--- Character Events ---")]

        public OnActiveRagdoll onActiveRagdoll = new OnActiveRagdoll();
        [Header("Check if Character is in Trigger with tag Action")]
        [HideInInspector]
        public OnActionHandle onActionEnter = new OnActionHandle();
        [HideInInspector]
        public OnActionHandle onActionStay = new OnActionHandle();
        [HideInInspector]
        public OnActionHandle onActionExit = new OnActionHandle();

        protected AnimatorParameter hitDirectionHash;
        protected AnimatorParameter reactionIDHash;
        protected AnimatorParameter triggerReactionHash;
        protected AnimatorParameter triggerResetStateHash;
        protected AnimatorParameter recoilIDHash;
        protected AnimatorParameter triggerRecoilHash;
        protected bool isInit;

        #endregion


        public virtual void Init()
        {
            animator = GetComponent<Animator>();
            if (animator)
            {
                hitDirectionHash = new AnimatorParameter(animator, "HitDirection");
                reactionIDHash = new AnimatorParameter(animator, "ReactionID");
                triggerReactionHash = new AnimatorParameter(animator, "TriggerReaction");
                triggerResetStateHash = new AnimatorParameter(animator, "ResetState");
                recoilIDHash = new AnimatorParameter(animator, "RecoilID");
                triggerRecoilHash = new AnimatorParameter(animator, "TriggerRecoil");
            }

            var actionListeners = GetComponents<vActions.vActionListener>();
            for (int i = 0; i < actionListeners.Length; i++)
            {
                if (actionListeners[i].actionEnter)
                    onActionEnter.AddListener(actionListeners[i].OnActionEnter);
                if (actionListeners[i].actionStay)
                    onActionStay.AddListener(actionListeners[i].OnActionStay);
                if (actionListeners[i].actionExit)
                    onActionExit.AddListener(actionListeners[i].OnActionExit);
            }
        }

        public virtual void ResetRagdoll()
        {

        }

        public virtual void EnableRagdoll()
        {

        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            onActionEnter.Invoke(other);
        }

        protected virtual void OnTriggerStay(Collider other)
        {
            onActionStay.Invoke(other);
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            onActionExit.Invoke(other);
        }

        public override void TakeDamage(vDamage damage)
        {
            base.TakeDamage(damage);
            TriggerDamageRection(damage);
        }

        protected virtual void TriggerDamageRection(vDamage damage)
        {
            if (animator != null && animator.enabled && !damage.activeRagdoll && currentHealth > 0)
            {
                if (hitDirectionHash.isValid) animator.SetInteger(hitDirectionHash, (int)transform.HitAngle(damage.sender.position));
                // trigger hitReaction animation
                if (damage.hitReaction)
                {
                    // set the ID of the reaction based on the attack animation state of the attacker - Check the MeleeAttackBehaviour script
                    if (reactionIDHash.isValid) animator.SetInteger(reactionIDHash, damage.reaction_id);
                    if (triggerReactionHash.isValid) animator.SetTrigger(triggerReactionHash);
                    if (triggerResetStateHash.isValid) animator.SetTrigger(triggerResetStateHash);
                }
                else
                {
                    if (recoilIDHash.isValid) animator.SetInteger(recoilIDHash, damage.recoil_id);
                    if (triggerRecoilHash.isValid) animator.SetTrigger(triggerRecoilHash);
                    if (triggerResetStateHash.isValid) animator.SetTrigger(triggerResetStateHash);
                }
            }
            if (damage.activeRagdoll) onActiveRagdoll.Invoke();
        }
    }
}