using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PartsKit
{
    public class RandomPointInCollider2D
    {
        private Collider2D mCollider;
        private int maxRandomCount = 1000;
        private Func<float, float, float> onRandom;

        private readonly List<Bounds> tempBounds = new List<Bounds>();

        public Bounds Bounds => mCollider.bounds;

        public RandomPointInCollider2D(Collider2D colliderVal)
        {
            mCollider = colliderVal;
        }

        public void SetMaxRandomCount(int count)
        {
            maxRandomCount = count;
        }

        public void SetRandomFun(Func<float, float, float> randomF)
        {
            onRandom = randomF;
        }

        public bool RandomPoint(out Vector3 point)
        {
            var bounds = mCollider.bounds;
            Vector3 minBound = bounds.min;
            Vector3 maxBound = bounds.max;
            return DoRandomPoint(minBound, maxBound, null, out point);
        }

        public bool RandomPointIn(Bounds inBounds, out Vector3 point)
        {
            var mapBounds = mCollider.bounds;
            if (!mapBounds.Intersection(inBounds, out var targetBounds))
            {
                point = Vector3.zero;
                return false;
            }

            Vector3 minBound = targetBounds.min;
            Vector3 maxBound = targetBounds.max;
            return DoRandomPoint(minBound, maxBound, null, out point);
        }

        public bool RandomPointIn(List<Bounds> inBounds, out Vector3 point)
        {
            var mapBounds = mCollider.bounds;
            tempBounds.Clear();
            tempBounds.AddRange(inBounds);
            tempBounds.Add(mapBounds);
            if (!inBounds.Intersection(out var targetBounds))
            {
                point = Vector3.zero;
                return false;
            }

            Vector3 minBound = targetBounds.min;
            Vector3 maxBound = targetBounds.max;
            return DoRandomPoint(minBound, maxBound, null, out point);
        }

        public bool RandomPointOut(Bounds outBounds, out Vector3 point)
        {
            var mapBounds = mCollider.bounds;
            Vector3 minBound = mapBounds.min;
            Vector3 maxBound = mapBounds.max;
            return DoRandomPoint(minBound, maxBound, (pos) => !outBounds.Contains(pos), out point);
        }

        public bool RandomPointOut(List<Bounds> outBounds, out Vector3 point)
        {
            var mapBounds = mCollider.bounds;
            Vector3 minBound = mapBounds.min;
            Vector3 maxBound = mapBounds.max;
            return DoRandomPoint(minBound, maxBound, (pos) =>
            {
                foreach (var bound in outBounds)
                {
                    if (bound.Contains(pos))
                    {
                        return false;
                    }
                }

                return true;
            }, out point);
        }

        public bool RandomPointInOut(Bounds inBounds, Bounds outBounds, out Vector3 point)
        {
            var mapBounds = mCollider.bounds;
            if (!mapBounds.Intersection(inBounds, out var targetBounds))
            {
                point = Vector3.zero;
                return false;
            }

            Vector3 minBound = targetBounds.min;
            Vector3 maxBound = targetBounds.max;
            return DoRandomPoint(minBound, maxBound, (pos) => !outBounds.Contains(pos), out point);
        }

        public bool RandomPointInOut(List<Bounds> inBounds, List<Bounds> outBounds, out Vector3 point)
        {
            var mapBounds = mCollider.bounds;
            tempBounds.Clear();
            tempBounds.AddRange(inBounds);
            tempBounds.Add(mapBounds);
            if (!tempBounds.Intersection(out var targetBounds))
            {
                point = Vector3.zero;
                return false;
            }

            Vector3 minBound = targetBounds.min;
            Vector3 maxBound = targetBounds.max;
            return DoRandomPoint(minBound, maxBound, (pos) =>
            {
                foreach (var bound in outBounds)
                {
                    if (bound.Contains(pos))
                    {
                        return false;
                    }
                }

                return true;
            }, out point);
        }

        public bool RandomPointInOut(Bounds inBounds, List<Bounds> outBounds, out Vector3 point)
        {
            var mapBounds = mCollider.bounds;
            if (!mapBounds.Intersection(inBounds, out var targetBounds))
            {
                point = Vector3.zero;
                return false;
            }

            Vector3 minBound = targetBounds.min;
            Vector3 maxBound = targetBounds.max;
            return DoRandomPoint(minBound, maxBound, (pos) =>
            {
                foreach (var bound in outBounds)
                {
                    if (bound.Contains(pos))
                    {
                        return false;
                    }
                }

                return true;
            }, out point);
        }

        public bool RandomPointInOut(List<Bounds> inBounds, Bounds outBounds, out Vector3 point)
        {
            var mapBounds = mCollider.bounds;
            tempBounds.Clear();
            tempBounds.AddRange(inBounds);
            tempBounds.Add(mapBounds);
            if (!tempBounds.Intersection(out var targetBounds))
            {
                point = Vector3.zero;
                return false;
            }

            Vector3 minBound = targetBounds.min;
            Vector3 maxBound = targetBounds.max;
            return DoRandomPoint(minBound, maxBound, (pos) => !outBounds.Contains(pos), out point);
        }

        private bool DoRandomPoint(Vector3 minBound, Vector3 maxBound, Func<Vector3, bool> checkFunc, out Vector3 point)
        {
            Vector3 randomPoint;
            int randomCount = 0;
            bool isSuccess = false;
            do
            {
                randomPoint =
                    new Vector3(
                        RandomValue(minBound.x, maxBound.x),
                        RandomValue(minBound.y, maxBound.y),
                        RandomValue(minBound.z, maxBound.z)
                    );
                randomCount++;
                if (mCollider.OverlapPoint(randomPoint) && (checkFunc == null || checkFunc.Invoke(randomPoint)))
                {
                    isSuccess = true;
                    break;
                }
            } while (randomCount <= maxRandomCount);

            point = randomPoint;
            return isSuccess;
        }

        private float RandomValue(float min, float max)
        {
            if (onRandom != null)
            {
                return onRandom.Invoke(min, max);
            }

            return Random.Range(min, max);
        }
    }
}