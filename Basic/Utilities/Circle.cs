using UnityEngine;

namespace PartsKit
{
    public struct Circle
    {
        public Vector3 Center { get; set; }
        public float Radius { get; set; }

        public Circle(Vector3 center, float radius)
        {
            Center = center;
            Radius = radius;
        }

        public bool Contains(Vector3 point)
        {
            float distanceSqr = (point - Center).sqrMagnitude;
            return distanceSqr <= Radius * Radius;
        }
    }
}