using UnityEngine;

namespace PartsKit
{
    public class FreezeTransform : MonoBehaviour
    {
        private Vector3 freezePosition;
        private Vector3 freezeLocalScale;
        private Quaternion freezeRotation;

        private void Update()
        {
            UpdateTransform();
        }

        public void Freeze(Transform target)
        {
            Freeze(target.position, target.localScale, target.rotation);
        }

        public void Freeze(Vector3 position, Vector3 localScale, Quaternion rotation)
        {
            freezePosition = position;
            freezeLocalScale = localScale;
            freezeRotation = rotation;
            UpdateTransform();
        }

        private void UpdateTransform()
        {
            var selfTrans = transform;
            selfTrans.position = freezePosition;
            selfTrans.localScale = freezeLocalScale;
            selfTrans.rotation = freezeRotation;
        }
    }
}