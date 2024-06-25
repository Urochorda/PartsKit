using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PartsKit
{
    public class RandomPointInCollider2D
    {
        private Collider2D collider;
        private Vector3 minBound;
        private Vector3 maxBound;
        private int maxRandomCount = 1000;
        private Func<float, float, float> onRandom;

        public RandomPointInCollider2D(Collider2D colliderVal)
        {
            SetCollider(colliderVal);
        }

        public void SetCollider(Collider2D colliderVal)
        {
            if (colliderVal == null)
            {
                collider = null;
                minBound = Vector3.zero;
                maxBound = Vector3.zero;
                return;
            }

            collider = colliderVal;
            var bounds = collider.bounds;
            minBound = bounds.min;
            maxBound = bounds.max;
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
            } while (!collider.OverlapPoint(randomPoint) && randomCount <= maxRandomCount);

            return randomPoint;
        }

        public float RandomValue(float min, float max)
        {
            if (onRandom != null)
            {
                return onRandom.Invoke(min, max);
            }

            return Random.Range(minBound.x, maxBound.x);
        }
    }
}