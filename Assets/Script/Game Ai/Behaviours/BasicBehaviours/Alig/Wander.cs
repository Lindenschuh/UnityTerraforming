using UnityEngine;

namespace UnityTerraforming.GameAi
{
    [RequireComponent(typeof(BehaviourRandom))]
    public class Wander : Face
    {
        public float Offset;
        public float Radius;
        public float Rate;

        [HideInInspector]
        public BehaviourRandom random;

        public override void Awake()
        {
            Target = new GameObject();
            Target.transform.position = transform.position;
            random = GetComponent<BehaviourRandom>();
            base.Awake();
        }

        public override Steering GetSteering()
        {
            Steering steering = new Steering();

            random.GetRandomRange(-1f, 1f);

            float wanderOrientation = random.CurrentRandom * Rate;
            float targetOrientation = wanderOrientation + Agent.Orientation;

            Vector3 orientationVec = OriAsVector(Agent.Orientation);
            Vector3 targetPosition = (Offset * orientationVec) + transform.position;

            targetPosition = targetPosition + (OriAsVector(targetOrientation) * Radius);
            targetAux.transform.position = targetPosition;

            steering = base.GetSteering();
            steering.linear = targetAux.transform.position - transform.position;
            steering.linear.Normalize();
            steering.linear *= Agent.MaxAccel;

            return steering;
        }
    }
}