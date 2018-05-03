using UnityEngine;
using System.Collections;
using System;
namespace Invector.vCharacterController
{
    using EventSystems;
    public partial class vCollisionMessage : MonoBehaviour, vIAttackReceiver
    {
        public float damageMultiplier = 1f;
        private vCharacter iChar;
        public void OnReceiveAttack(vDamage damage, vIMeleeFighter attacker)
        {
            if (overrideReactionID) damage.reaction_id = reactionID;
            if (ragdoll && !ragdoll.iChar.isDead)
            {
                var _damage = new vDamage(damage);
                var value = (float)_damage.damageValue;
                _damage.damageValue = (int)(value * damageMultiplier);
                ragdoll.gameObject.ApplyDamage(_damage, attacker);
            }
            else
            {
                if (!iChar) iChar = GetComponentInParent<vCharacter>();
                if (iChar)
                {
                    var _damage = new vDamage(damage);
                    var value = (float)_damage.damageValue;
                    _damage.damageValue = (int)(value * damageMultiplier);
                    iChar.gameObject.ApplyDamage(_damage, attacker);
                }
            }
        }
    }

}
