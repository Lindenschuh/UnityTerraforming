using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTerraforming.GameAi
{
    [RequireComponent(typeof(Animator))]
    public class Agent : MonoBehaviour
    {
        public float ModelOffset;

        public float MaxSpeed;
        public float MaxAccel;
        public float MaxRotation;
        public float MaxAngularAccel;

        private Animator _animator;

        public int Damage = 2;

        [HideInInspector]
        public float Rotation;

        [HideInInspector]
        public float Orientation;

        [HideInInspector]
        public Vector3 Velocity;

        [HideInInspector]
        public bool Attacking;

        [HideInInspector]
        public bool Dying;

        protected Steering steering;

        private void Start()
        {
            _animator = GetComponent<Animator>();
            Velocity = Vector3.zero;
            steering = new Steering();
        }

        public void SetSteering(Steering steering, float weight = 1f)
        {
            this.steering.linear += (weight * steering.linear);
            this.steering.angualr += (weight * steering.angualr);
        }

        public void FixedUpdate()
        {
            if (!Attacking && !Dying)
            {
                Vector3 displacement = Velocity * Time.deltaTime;
                if (Velocity.magnitude > 0)
                    transform.rotation = Quaternion.LookRotation(Velocity);

                if (Orientation < 0)
                    Orientation += 360f;
                else if (Orientation > 360f)
                    Orientation -= 360f;

                transform.Translate(displacement, Space.World);
                UpdateAnimation();

                Velocity += steering.linear * Time.deltaTime;
                Rotation += steering.angualr * Time.deltaTime;

                if (Velocity.magnitude > MaxSpeed)
                {
                    Velocity.Normalize();
                    Velocity = Velocity * MaxSpeed;
                }
                if (steering.angualr == 0)
                {
                    Rotation = 0;
                }

                if (Velocity.y != 0)
                {
                    Velocity.y = 0;
                }

                if (steering.linear.sqrMagnitude == 0)
                {
                    Velocity = Vector3.zero;
                }
            }
            else
            {
                UpdateAnimation();
            }
            steering = new Steering();
        }

        private void UpdateAnimation()
        {
            _animator.SetFloat("WalkSpeed", Velocity.magnitude / MaxSpeed);
            _animator.SetBool("Attacking", Attacking);
            _animator.SetBool("Dying", Dying);
        }
    }
}