using UnityEngine;
using System.Collections;
using System;
namespace Invector.vCharacterController
{
    using EventSystems;
    public partial class vCollisionMessage : MonoBehaviour, vIDamageReceiver
    {
        [HideInInspector]
        public vRagdoll ragdoll;
        public bool overrideReactionID;
        [vHideInInspector("overrideReactionID")]
        public int reactionID;

        void Start()
        {
            ragdoll = GetComponentInParent<vRagdoll>();
        }

        void OnCollisionEnter(Collision collision)
        {
            if (collision != null)
            {
                if (ragdoll && ragdoll.isActive)
                {
                    ragdoll.OnRagdollCollisionEnter(new vRagdollCollision(this.gameObject, collision));
                    if (!inAddDamage)
                    {
                        float impactforce = collision.relativeVelocity.x + collision.relativeVelocity.y + collision.relativeVelocity.z;
                        if (impactforce > 10 || impactforce < -10)
                        {
                            inAddDamage = true;
                            vDamage damage = new vDamage((int)Mathf.Abs(impactforce) - 10);
                            damage.ignoreDefense = true;
                            damage.sender = collision.transform;
                            damage.hitPosition = collision.contacts[0].point;

                            Invoke("ResetAddDamage", 0.1f);
                        }
                    }
                }
            }
        }

        bool inAddDamage;

        void ResetAddDamage()
        {
            inAddDamage = false;
        }

        public void TakeDamage(vDamage damage)
        {
            if (!ragdoll) return;
            if (!ragdoll.iChar.isDead)
            {
                inAddDamage = true;
                if (overrideReactionID) damage.reaction_id = reactionID;

                ragdoll.ApplyDamage(damage);
                Invoke("ResetAddDamage", 0.1f);
            }
        }
    }
}