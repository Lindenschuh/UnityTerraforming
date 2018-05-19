using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace UnityTerraforming.GameAi
{
    public class PathFinding : MonoBehaviour
    {
        public List<GameObject> nodes;
        private List<PathSegment> segments;

        private void Start()
        {
            segments = GetSegments();
        }

        private List<PathSegment> GetSegments()
        {
            List<PathSegment> segments = new List<PathSegment>();
            for (int i = 0; i < nodes.Count - 1; i++)
            {
                Vector3 src = nodes[i].transform.position;
                Vector3 dst = nodes[i + 1].transform.position;
                PathSegment segment = new PathSegment(src, dst);
                segments.Add(segment);
            }
            return segments;
        }

        public float GetParam(Vector3 position, float lastParam)
        {
            float param = 0;
            PathSegment currentSegment = null;
            float tempParam = 0;
            foreach (PathSegment ps in segments)
            {
                tempParam += Vector3.Distance(ps.a, ps.b);
                if (lastParam <= tempParam)
                {
                    currentSegment = ps;
                    break;
                }
            }
            if (currentSegment == null)
                return 0;

            Vector3 currPos = position - currentSegment.a;
            Vector3 segmentDirection = currentSegment.b - currentSegment.a;
            segmentDirection.Normalize();

            Vector3 pointInSegment = Vector3.Project(currPos, segmentDirection);
            param += pointInSegment.magnitude;
            return param;
        }

        public Vector3 GetPosition(float param)
        {
            Vector3 position = Vector3.zero;

            PathSegment currentSegment = null;
            float tempParam = 0;
            foreach (PathSegment ps in segments)
            {
                tempParam += Vector3.Distance(ps.a, ps.b);
                if (param <= tempParam)
                {
                    currentSegment = ps;
                    break;
                }
            }
            if (currentSegment == null)
                return Vector3.zero;

            Vector3 segmentDirection = currentSegment.b - currentSegment.a;
            segmentDirection.Normalize();
            tempParam -= Vector3.Distance(currentSegment.a, currentSegment.b);
            tempParam = param - tempParam;
            position = currentSegment.a + segmentDirection * tempParam;
            return position;
        }

        private void OnDrawGizmos()
        {
            Vector3 direction;
            Color tmp = Gizmos.color;
            Gizmos.color = Color.magenta;
            for (int i = 0; i < nodes.Count - 1; i++)
            {
                Vector3 src = nodes[i].transform.position;
                Vector3 dst = nodes[i + 1].transform.position;
                direction = dst - src;
                Gizmos.DrawRay(src, direction);
            }
            Gizmos.color = tmp;
        }
    }
}