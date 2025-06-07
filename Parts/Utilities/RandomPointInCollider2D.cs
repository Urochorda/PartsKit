using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PartsKit
{
    public class RandomPointInCollider2D
    {
        private Collider2D mCollider;
        private int maxRandomCount = 20;
        private Func<float, float, float> onRandom;

        private readonly List<Bounds> tempBounds = new List<Bounds>();

        public Bounds Bounds => mCollider.bounds.To2D();

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

        private bool DoRandomPoint(Vector3 minBound, Vector3 maxBound, Func<Vector3, bool> checkFunc, out Vector3 point)
        {
            Vector2 randomPoint; //2d的所以z轴不处理，也就是0
            int randomCount = 0;
            bool isSuccess = false;
            do
            {
                randomPoint = new Vector2(RandomValue(minBound.x, maxBound.x), RandomValue(minBound.y, maxBound.y));
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

        public bool RandomPoint(out Vector3 point)
        {
            var mapBounds = Bounds;
            Vector3 minBound = mapBounds.min;
            Vector3 maxBound = mapBounds.max;
            return DoRandomPoint(minBound, maxBound, null, out point);
        }

        #region In

        public bool RandomPointIn(Bounds inBounds, out Vector3 point)
        {
            var mapBounds = Bounds;
            inBounds = inBounds.To2D();
            if (!mapBounds.Intersection(inBounds, out var targetBounds))
            {
                point = Vector3.zero;
                return false;
            }

            Vector3 minBound = targetBounds.min;
            Vector3 maxBound = targetBounds.max;
            return DoRandomPoint(minBound, maxBound, null, out point);
        }

        public bool RandomPointIn(Circle circle, out Vector3 point)
        {
            //因为是2d，所有z给0
            circle = circle.To2D();
            var inBounds = new Bounds(circle.Center, new Vector3(circle.Radius, circle.Radius, 0)).To2D();
            var mapBounds = Bounds;
            if (!mapBounds.Intersection(inBounds, out var targetBounds))
            {
                point = Vector3.zero;
                return false;
            }

            Vector3 minBound = targetBounds.min;
            Vector3 maxBound = targetBounds.max;

            return DoRandomPoint(minBound, maxBound, (pos) => circle.Contains2D(pos), out point);
        }

        #endregion

        #region Out

        public bool RandomPointOut(Bounds outBounds, out Vector3 point)
        {
            var mapBounds = Bounds;
            outBounds = outBounds.To2D();
            if (outBounds.Contains(mapBounds))
            {
                point = Vector3.zero;
                return false;
            }

            Vector3 minBound = mapBounds.min;
            Vector3 maxBound = mapBounds.max;
            return DoRandomPoint(minBound, maxBound, (pos) => !outBounds.Contains2D(pos), out point);
        }

        public bool RandomPointOut(List<Bounds> outBounds, out Vector3 point)
        {
            var mapBounds = Bounds;
            tempBounds.Clear();
            tempBounds.AddRange(outBounds);
            tempBounds.To2D();
            foreach (var outBound in tempBounds)
            {
                if (outBound.Contains(mapBounds))
                {
                    point = Vector3.zero;
                    return false;
                }
            }

            Vector3 minBound = mapBounds.min;
            Vector3 maxBound = mapBounds.max;
            return DoRandomPoint(minBound, maxBound, (pos) =>
            {
                foreach (var bound in tempBounds)
                {
                    if (bound.Contains2D(pos))
                    {
                        return false;
                    }
                }

                return true;
            }, out point);
        }

        public bool RandomPointOut(Circle circle, out Vector3 point)
        {
            var mapBounds = Bounds;
            circle = circle.To2D();
            if (circle.Contains(mapBounds))
            {
                point = Vector3.zero;
                return false;
            }

            Vector3 minBound = mapBounds.min;
            Vector3 maxBound = mapBounds.max;
            return DoRandomPoint(minBound, maxBound, (pos) => !circle.Contains2D(pos), out point);
        }

        #endregion

        #region InOut

        public bool RandomPointInOut(Bounds inBounds, Bounds outBounds, out Vector3 point)
        {
            var mapBounds = Bounds;
            inBounds = inBounds.To2D();
            outBounds = outBounds.To2D();
            if (outBounds.Contains(mapBounds) || outBounds.Contains(inBounds))
            {
                point = Vector3.zero;
                return false;
            }

            if (!mapBounds.Intersection(inBounds, out var targetBounds))
            {
                point = Vector3.zero;
                return false;
            }

            Vector3 minBound = targetBounds.min;
            Vector3 maxBound = targetBounds.max;
            return DoRandomPoint(minBound, maxBound, (pos) => !outBounds.Contains2D(pos), out point);
        }

        public bool RandomPointInOut(Bounds inBounds, List<Bounds> outBounds, out Vector3 point)
        {
            var mapBounds = Bounds;
            if (!mapBounds.Intersection(inBounds, out var targetBounds))
            {
                point = Vector3.zero;
                return false;
            }

            tempBounds.Clear();
            tempBounds.AddRange(outBounds);
            tempBounds.To2D();
            foreach (var outBound in tempBounds)
            {
                if (outBound.Contains(mapBounds) || outBound.Contains(inBounds))
                {
                    point = Vector3.zero;
                    return false;
                }
            }

            Vector3 minBound = targetBounds.min;
            Vector3 maxBound = targetBounds.max;
            return DoRandomPoint(minBound, maxBound, (pos) =>
            {
                foreach (var bound in tempBounds)
                {
                    if (bound.Contains2D(pos))
                    {
                        return false;
                    }
                }

                return true;
            }, out point);
        }

        #endregion
    }
}