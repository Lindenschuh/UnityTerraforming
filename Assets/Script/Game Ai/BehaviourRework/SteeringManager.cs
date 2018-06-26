using UnityEngine;
using System.Collections.Generic;

namespace UnityTerraforming.GameAi
{
    public enum SteeringTypes
    {
        ATTACK,
        SEEK,
        FLEE,
        ARRIVE,
        LEAVE,
        AVOID_WALLS,
        AVOID_AGENTS,
        WANDER
    }

    public static class SteeringManager
    {
        private const float TIME_TO_TARGET = 0.1f;

        public static Steering GetSeek(Agent agent, Vector3 targetPosition)
        {
            return new Steering()
            {
                linear = (targetPosition - agent.transform.position).normalized * agent.MaxAccel
            };
        }

        public static Steering GetFlee(Agent agent, Vector3 targetPosition)
        {
            return new Steering()
            {
                linear = -((targetPosition + agent.transform.position).normalized * agent.MaxAccel)
            };
        }

        public static Steering GetArrive(Agent agent, Vector3 targetPosition, float targetRadius, float slowRadius)
        {
            Steering steering = new Steering();
            Vector3 direction = targetPosition - agent.transform.position;
            float distance = direction.magnitude;

            // If inside the Target Radius return the empty Steering
            if (distance < targetRadius) return steering;

            float targetSpeed;
            if (distance > slowRadius)
                targetSpeed = agent.MaxSpeed;
            else
                targetSpeed = agent.MaxSpeed * distance / slowRadius;

            Vector3 desiredVelocity = direction;
            desiredVelocity.Normalize();
            desiredVelocity *= targetSpeed;

            steering.linear = desiredVelocity - agent.Velocity;
            steering.linear /= TIME_TO_TARGET;

            steering.linear = Vector3.ClampMagnitude(steering.linear, agent.MaxAccel);

            return steering;
        }

        public static Steering GetLeave(Agent agent, Vector3 targetPosition, float dangerRadius, float escapeRadius)
        {
            Steering steering = new Steering();
            Vector3 direction = agent.transform.position - targetPosition;
            float distance = direction.magnitude;

            if (distance > dangerRadius)
                return steering;

            float reduce = (distance < escapeRadius) ? 0 : distance / dangerRadius * agent.MaxSpeed;

            float targetSpeed = agent.MaxSpeed - reduce;

            Vector3 desiredVelocity = direction.normalized * targetSpeed;

            steering.linear = (desiredVelocity - agent.Velocity) / TIME_TO_TARGET;

            steering.linear = Vector3.ClampMagnitude(steering.linear, agent.MaxAccel);

            return steering;
        }

        public static Steering GetAvoidWalls(Agent agent, float lookAhead, float avoidDistance, LayerMask mask, float feelerAngle = 45, float feelerScale = 2)
        {
            Steering steering = new Steering();
            Vector3 position = agent.transform.position;
            Vector3 rayVector = agent.Velocity.normalized * lookAhead;
            Vector3 direction = rayVector;
            Vector3 directionRight = Quaternion.AngleAxis(feelerAngle, Vector3.up) * rayVector;
            Vector3 directionLeft = Quaternion.AngleAxis(-feelerAngle, Vector3.up) * rayVector;

            RaycastHit hit;

            Vector3 target = new Vector3();

            Debug.DrawRay(position, direction, Color.green);
            Debug.DrawRay(position, directionLeft / feelerScale, Color.red);
            Debug.DrawRay(position, directionRight / feelerScale, Color.blue);

            if (Physics.Raycast(position, direction, out hit, lookAhead, mask))
            {
                target = hit.point + hit.normal * avoidDistance; ;
                Debug.DrawRay(hit.point, target, Color.yellow);
                return GetSeek(agent, target);
            }
            if (Physics.Raycast(position, directionLeft, out hit, lookAhead / feelerScale, mask))
            {
                target = hit.point + hit.normal * avoidDistance; ;
                Debug.DrawRay(hit.point, target, Color.yellow);
                return GetSeek(agent, target);
            }

            if (Physics.Raycast(position, directionRight, out hit, lookAhead / feelerScale, mask))
            {
                target = hit.point + hit.normal * avoidDistance; ;
                Debug.DrawRay(hit.point, target, Color.yellow);
                return GetSeek(agent, target);
            }

            return steering;
        }

        public static Steering GetWander(Agent agent, float offset, float radius, float rate)
        {
            float wanderOrientation = Random.Range(-rate, +rate);

            Vector3 currentDirection = OriAsVector(wanderOrientation);
            Vector3 targetPosition = (offset * currentDirection) + agent.transform.position;

            return GetSeek(agent, targetPosition);
        }

        public static Steering GetAvoidAgents(Agent agent, List<Agent> targets, float collisionRadius)
        {
            Steering steering = new Steering();

            float shortestTime = Mathf.Infinity;
            Agent firstTarget = null;
            float firstMinSeperation = 0;
            float firstDistance = 0;
            Vector3 firstRelativePos = Vector3.zero;
            Vector3 firstRelativeVel = Vector3.zero;

            foreach (Agent t in targets)
            {
                Vector3 relativePos = t.transform.position - agent.transform.position;
                Vector3 relativeVel = t.Velocity - agent.Velocity;
                float relativeSpeed = relativeVel.magnitude;
                float timeToCollision = Vector3.Dot(relativePos, relativeVel) / (relativeSpeed * relativeSpeed * -1);
                float distance = relativePos.magnitude;
                float minSeperation = distance - relativeSpeed * timeToCollision;
                if (minSeperation > 2 * collisionRadius)
                    continue;

                if (timeToCollision > 0 && timeToCollision < shortestTime)
                {
                    shortestTime = timeToCollision;
                    firstTarget = t;
                    firstMinSeperation = minSeperation;
                    firstRelativePos = relativePos;
                    firstRelativeVel = relativeVel;
                }
            }

            if (firstTarget == null)
                return steering;

            if (firstMinSeperation <= 0 || firstDistance < 2 * collisionRadius)
                firstRelativePos = firstTarget.transform.position;
            else
                firstRelativePos += firstRelativeVel * shortestTime;

            steering.linear = -firstRelativePos.normalized * agent.MaxAccel;

            return steering;
        }

        public static Vector3 OriAsVector(float orientation)
        {
            Vector3 vector = Vector3.zero;
            vector.x = Mathf.Sin(orientation * Mathf.Deg2Rad) * 1f;
            vector.z = Mathf.Cos(orientation * Mathf.Deg2Rad) * 1f;
            return vector.normalized;
        }
    }
}