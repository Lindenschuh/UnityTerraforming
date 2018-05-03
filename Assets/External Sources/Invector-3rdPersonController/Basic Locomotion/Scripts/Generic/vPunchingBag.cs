﻿using UnityEngine;
using System.Collections;

namespace Invector.vCharacterController
{
    public class vPunchingBag : MonoBehaviour
    {
        public Rigidbody _rigidbody;
        public float forceMultipler = 0.5f;
        public SpringJoint joint;
        public vCharacter character;

        void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            character = GetComponent<vCharacter>();
            character.onReceiveDamage.AddListener(TakeDamage);
        }       
        public void TakeDamage(vDamage damage)
        {
            var point = damage.hitPosition;
            var relativePoint = transform.position;
            relativePoint.y = point.y;
            var forceForward = relativePoint - point;

            if (character != null && joint != null && character.currentHealth < 0)
            {
                joint.connectedBody = null;
                if (character.removeComponentsAfterDie)
                {
                    foreach (MonoBehaviour mono in character.gameObject.GetComponentsInChildren<MonoBehaviour>())
                        if (mono != this)
                            Destroy(mono);
                }
            }

            if (_rigidbody != null)
            {
                _rigidbody.AddForce(forceForward * (damage.damageValue * forceMultipler), ForceMode.Impulse);
            }
        }
    }
}