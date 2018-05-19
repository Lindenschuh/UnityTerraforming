using UnityEngine;

namespace UnityTerraforming.GameAi
{
    public class Face : Align
    {
        protected GameObject targetAux;

        public override void Awake()
        {
            base.Awake();
            targetAux = Target;
            Target = new GameObject();
            Target.AddComponent<Agent>();
        }

        public override Steering GetSteering()
        {
            Vector3 direction = targetAux.transform.position - transform.position;

            if (direction.magnitude > 0)
            {
                float targetOrientation = Mathf.Atan2(direction.x, direction.z);
                targetOrientation *= Mathf.Rad2Deg;
                Target.GetComponent<Agent>().Orientation = targetOrientation;
            }
            return base.GetSteering();
        }

        private void OnDestroy()
        {
            Destroy(Target);
        }
    }
}