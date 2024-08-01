using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PartsKit
{
    public class RandomPointInCollider2D
    {
        private Collider2D mCollider;
        private int maxRandomCount = 1000;
        private Func<float, float, float> onRandom;

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

        public Vector3 RandomPoint()
        {
            var bounds = mCollider.bounds;
            Vector3 minBound = bounds.min;
            Vector3 maxBound = bounds.max;
            Vector3 randomPoint;
            int randomCount = 0;
            do
            {
                randomPoint =
                    new Vector3(
                        RandomValue(minBound.x, maxBound.x),
                        RandomValue(minBound.y, maxBound.y),
                        RandomValue(minBound.z, maxBound.z)
                    );
                randomCount++;
            } while (!mCollider.OverlapPoint(randomPoint) && randomCount <= maxRandomCount);

            return randomPoint;
        }

        public float RandomValue(float min, float max)
        {
            if (onRandom != null)
            {
                return onRandom.Invoke(min, max);
            }

            return Random.Range(min, max);
        }
    }
}