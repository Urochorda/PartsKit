using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PartsKit
{
    public class RandomPointInCollider2D
    {
        private readonly Collider2D mCollider;
        private int maxRandomCount = 20;
        private Func<float, float, float> onRandom;

        private readonly List<Bounds> tempBounds = new List<Bounds>();
        private readonly List<Bounds> tempBounds2 = new List<Bounds>();

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
                randomCount++;
                randomPoint = new Vector2(RandomValue(minBound.x, maxBound.x), RandomValue(minBound.y, maxBound.y));
                if (mCollider.OverlapPoint(randomPoint) && (checkFunc == null || checkFunc.Invoke(randomPoint)))
                {
                    isSuccess = true;
                    break;
                }
            } while (randomCount <= maxRandomCount);

            point = randomPoint;
            return isSuccess;
        }

        private bool DoRandomPoint(List<Bounds> validBounds, Func<Vector3, bool> checkFunc, out Vector3 point)
        {
            point = Vector3.zero;

            if (validBounds == null || validBounds.Count == 0)
            {
                return false;
            }

            float totalArea = 0f;
            var areas = new float[validBounds.Count];
            for (var i = 0; i < validBounds.Count; i++)
            {
                var bound = validBounds[i];
                float area = Mathf.Max(0f, bound.size.x * bound.size.y);
                areas[i] = area;
                totalArea += area;
            }

            if (totalArea <= 0f)
            {
                return false;
            }

            bool isSuccess = false;
            int checkCount = areas.Length;
            do
            {
                float rand = RandomValue(0, totalArea);
                float weightSum = 0f;
                int chosenIndex = 0;
                Bounds chosenBound = validBounds[chosenIndex];

                for (int i = 0; i < areas.Length; i++)
                {
                    weightSum += areas[i];
                    if (rand <= weightSum)
                    {
                        chosenIndex = i;
                        chosenBound = validBounds[i];
                        break;
                    }
                }

                Vector3 minBound = chosenBound.min;
                Vector3 maxBound = chosenBound.max;
                if (!DoRandomPoint(minBound, maxBound, checkFunc, out point))
                {
                    totalArea -= areas[chosenIndex];
                    areas[chosenIndex] = 0;
                    checkCount--;
                }
                else
                {
                    isSuccess = true;
                    break;
                }
            } while (checkCount > 0);

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
            var inBounds = new Bounds(circle.Center, new Vector3(circle.Radius * 2, circle.Radius * 2, 0)).To2D();
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
            mapBounds.SubtractBounds(outBounds, tempBounds, true);
            return DoRandomPoint(tempBounds, null, out point);
        }

        public bool RandomPointOut(List<Bounds> outBounds, out Vector3 point)
        {
            var mapBounds = Bounds;
            tempBounds2.Clear();
            tempBounds2.AddRange(outBounds);
            tempBounds2.To2D();
            mapBounds.SubtractBounds(tempBounds2, tempBounds, true);
            return DoRandomPoint(tempBounds, null, out point);
        }

        #endregion

        #region InOut

        public bool RandomPointInOut(Bounds inBounds, Bounds outBounds, out Vector3 point)
        {
            var mapBounds = Bounds;
            inBounds = inBounds.To2D();
            if (!mapBounds.Intersection(inBounds, out var targetBounds))
            {
                point = Vector3.zero;
                return false;
            }

            outBounds = outBounds.To2D();
            targetBounds.SubtractBounds(outBounds, tempBounds, true);
            return DoRandomPoint(tempBounds, null, out point);
        }

        public bool RandomPointInOut(Bounds inBounds, List<Bounds> outBounds, out Vector3 point)
        {
            var mapBounds = Bounds;
            if (!mapBounds.Intersection(inBounds, out var targetBounds))
            {
                point = Vector3.zero;
                return false;
            }

            tempBounds2.Clear();
            tempBounds2.AddRange(outBounds);
            tempBounds2.To2D();
            targetBounds.SubtractBounds(tempBounds2, tempBounds, true);
            return DoRandomPoint(tempBounds, null, out point);
        }

        #endregion
    }
}