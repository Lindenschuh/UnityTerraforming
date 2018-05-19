using UnityEngine;

namespace UnityTerraforming.GameAi
{
    public class PathFollwoer : Seek
    {
        public PathFinding Path;
        public float PathOffset = 0f;
        private float currentParam;

        public override void Awake()
        {
            base.Awake();
            Target = new GameObject();
            currentParam = 0f;
        }

        public override Steering GetSteering()
        {
            currentParam = Path.GetParam(transform.position, currentParam);
            float targetParam = currentParam + PathOffset;
            Target.transform.position = Path.GetPosition(targetParam);

            return base.GetSteering();
        }
    }
}