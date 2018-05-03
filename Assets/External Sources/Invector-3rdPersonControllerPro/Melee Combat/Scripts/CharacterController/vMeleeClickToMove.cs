using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Invector.vCharacterController.ClickToMove
{
    using vMelee;
    using EventSystems;

    public class vMeleeClickToMove : vClickToMoveInput, vIMeleeFighter
    {
        vMeleeManager meleeManager;      

        protected override void Start()
        {
            base.Start();
            meleeManager = gameObject.GetComponent<vMeleeManager>();
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            UpdateMeleeAnimations();
            UpdateAttackBehaviour();
        }

        public override void MoveCharacter(Vector3 position, bool rotateToDirection = true)
        {                   
            if(target && meleeManager.hitProperties.hitDamageTags.Contains(target.gameObject.tag))
            {
                if (Physics.Raycast(cc._capsuleCollider.bounds.center, (target.bounds.center - cc._capsuleCollider.bounds.center).normalized, meleeManager.GetAttackDistance()))
                {
                    RotateTo((target.bounds.center - cc._capsuleCollider.bounds.center).normalized);
                    ClearTarget();
                    TriggerAttack();
                }
                else
                {
                    base.MoveCharacter(position, rotateToDirection);
                }
            }
            else
            {
                base.MoveCharacter(position, rotateToDirection);
            }
        }

        protected virtual void TriggerAttack()
        {
            if (MeleeAttackStaminaConditions())
            {
                animator.SetInteger("AttackID", meleeManager.GetAttackID());
                animator.SetTrigger("WeakAttack");
            }
        }

        protected virtual void RotateTo(Vector3 direction)
        {
            direction.y = 0;
            transform.rotation = Quaternion.LookRotation(direction, transform.up);
        }

        protected virtual bool MeleeAttackStaminaConditions()
        {
            var result = cc.currentStamina - meleeManager.GetAttackStaminaCost();
            return result >= 0;
        }

        #region Update Animations

        protected virtual void UpdateMeleeAnimations()
        {
            if (cc.animator == null || meleeManager == null) return;
            cc.animator.SetInteger("AttackID", meleeManager.GetAttackID());
            cc.animator.SetInteger("DefenseID", meleeManager.GetDefenseID());
            cc.animator.SetBool("IsBlocking", isBlocking);
            cc.animator.SetFloat("MoveSet_ID", meleeManager.GetMoveSetID(), .2f, Time.deltaTime);
        }

        protected virtual void UpdateAttackBehaviour()
        {
            if (cc.IsAnimatorTag("Attack")) return;
            // lock the speed to stop the character from moving while attacking
            cc.lockSpeed = cc.IsAnimatorTag("Attack") || isAttacking;
            // force root motion animation while attacking
            cc.forceRootMotion = cc.IsAnimatorTag("Attack") || isAttacking;
        }
        
        #endregion

        #region Melee Interface

        public bool isBlocking { get; set; }

        public bool isAttacking { get; set; }

        public bool isArmed { get; set; }

        public vCharacter character { get { return cc; } }

        public void OnEnableAttack()
        {
            cc.currentStaminaRecoveryDelay = meleeManager.GetAttackStaminaRecoveryDelay();
            cc.currentStamina -= meleeManager.GetAttackStaminaCost();
            cc.lockRotation = true;
            isAttacking = true;
        }

        public void OnDisableAttack()
        {
            cc.lockRotation = false;
            isAttacking = false;
        }

        public void ResetAttackTriggers()
        {
            cc.animator.ResetTrigger("WeakAttack");
            cc.animator.ResetTrigger("StrongAttack");
        }

        public void BreakAttack(int breakAtkID)
        {
            ResetAttackTriggers();
            OnRecoil(breakAtkID);
        }

        public void OnRecoil(int recoilID)
        {
            cc.animator.SetInteger("RecoilID", recoilID);
            cc.animator.SetTrigger("TriggerRecoil");
            cc.animator.SetTrigger("ResetState");
            cc.animator.ResetTrigger("WeakAttack");
            cc.animator.ResetTrigger("StrongAttack");
        }

        public void OnReceiveAttack(vDamage damage, vIMeleeFighter attacker)
        {
            // character is blocking
            if (!damage.ignoreDefense && isBlocking && meleeManager != null && meleeManager.CanBlockAttack(attacker.character.transform.position))
            {
                var damageReduction = meleeManager.GetDefenseRate();
                if (damageReduction > 0)
                    damage.ReduceDamage(damageReduction);
                if (attacker != null && meleeManager != null && meleeManager.CanBreakAttack())
                    attacker.OnRecoil(meleeManager.GetDefenseRecoilID());
                meleeManager.OnDefense();
                cc.currentStaminaRecoveryDelay = damage.staminaRecoveryDelay;
                cc.currentStamina -= damage.staminaBlockCost;
            }
            // apply damage
            damage.hitReaction = !isBlocking;
            cc.TakeDamage(damage);
        }

        #endregion
    }

}
