using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTerraforming.GameAi
{
    public class Agent : MonoBehaviour
    {
        public float MaxSpeed;
        public float MaxAccel;
        public float MaxRotation;
        public float MaxAngularAccel;
        public float Orientation;

        public int Damage = 2;

        [HideInInspector]
        public float Rotation;

        [HideInInspector]
        public Vector3 Velocity;

        protected Steering steering;

        private void Start()
        {
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
            Vector3 displacement = Velocity * Time.deltaTime;
            if (Velocity.magnitude > 0)
                transform.rotation = Quaternion.LookRotation(Velocity);

            if (Orientation < 0)
                Orientation += 360f;
            else if (Orientation > 360f)
                Orientation -= 360f;

            transform.Translate(displacement, Space.World);

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

            steering = new Steering();
        }
    }
}