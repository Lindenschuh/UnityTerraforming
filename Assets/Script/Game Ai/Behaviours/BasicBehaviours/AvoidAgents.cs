using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTerraforming.GameAi
{
    public class AvoidAgents : AgentBehaviour
    {
        public float ColisionRadius = 0.4f;
        public float ScanRadius;
        public float ScanTime = 0.5f;
        private List<Agent> targets;

        private void Start()
        {
            targets = new List<Agent>();
            StartCoroutine("SearchTargets", ScanTime);
        }

        public IEnumerable SearchTargets(float timeToWait)
        {
            while (true)
            {
                SearchTargets();
                yield return new WaitForSeconds(timeToWait);
            }
        }

        private void SearchTargets()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, ScanRadius);
            foreach (Collider col in colliders)
            {
                Agent agent = col.GetComponent<Agent>();
                if (agent != null)
                {
                    targets.Add(agent);
                }
            }
        }

        public override Steering GetSteering()
        {
            Steering steering = new Steering();
            float shortestTime = Mathf.Infinity;
            Agent firstTarget = null;
            float firstMinSeperaton = 0;
            float firstDistance = 0;
            Vector3 firstRelativePos = Vector3.zero;
            Vector3 firstRelativeVel = Vector3.zero;

            foreach (Agent t in targets)
            {
                Vector3 relativePos;
                relativePos = t.transform.position - transform.position;
                Vector3 relativeVel = t.Velocity - Agent.Velocity;
                float relativeSpeed = relativeVel.magnitude;
                float timeToCollision = Vector3.Dot(relativePos, relativeVel);
                timeToCollision /= relativeSpeed * relativeSpeed * -1;
                float distance = relativePos.magnitude;
                float minSeperation = distance - relativeSpeed * timeToCollision;
                if (minSeperation > 2 * ColisionRadius)
                    continue;
                if (timeToCollision > 0 && timeToCollision < shortestTime)
                {
                    shortestTime = timeToCollision;
                    firstTarget = t;
                    firstMinSeperaton = minSeperation;
                    firstRelativePos = relativePos;
                    firstRelativeVel = relativeVel;
                }
            }

            if (firstTarget == null)
                return steering;

            if (firstMinSeperaton <= 0 || firstDistance < 2 * ColisionRadius)
                firstRelativePos = firstTarget.transform.position;
            else
                firstRelativePos += firstRelativeVel * shortestTime;

            firstRelativePos.Normalize();
            steering.linear = -firstRelativePos * Agent.MaxAccel;

            return steering;
        }
    }
}