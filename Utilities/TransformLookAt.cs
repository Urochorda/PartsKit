using UnityEngine;

namespace PartsKit
{
    public class TransformLookAt : MonoBehaviour
    {
        enum LookAtIgnore
        {
            Null,
            X,
            Y,
            Z,
        }

        enum LookAtScale
        {
            Null,
            X,
            Y,
            Z,
        }

        enum LookAtDir
        {
            Forward,
            Right,
            Up
        }

        [SerializeField] private CheckNullProperty<Transform> target;
        [Header("Rotate")] [SerializeField] private LookAtDir lookAtDir = LookAtDir.Right;
        [SerializeField] private LookAtIgnore lookAtIgnore = LookAtIgnore.Z;
        [Header("Scale")] [SerializeField] private LookAtScale lookAtScaleCompare = LookAtScale.X;
        [SerializeField] private Vector3 lookAtDefaultScale = Vector3.one;
        [SerializeField] private Vector3 lookAtCompareScale = new Vector3(1, -1, 1);

        public void SetTarget(Transform targetTran)
        {
            target.SetData(targetTran);
        }

        private void Update()
        {
            if (!target.GetValue(out Transform targetTransform))
            {
                return;
            }

            Transform thisTransform = transform;
            Vector3 direction = targetTransform.position - thisTransform.position;
            switch (lookAtIgnore)
            {
                case LookAtIgnore.X:
                    direction.x = 0;
                    break;
                case LookAtIgnore.Y:
                    direction.y = 0;
                    break;
                case LookAtIgnore.Z:
                    direction.z = 0;
                    break;
            }

            switch (lookAtDir)
            {
                case LookAtDir.Forward:
                    thisTransform.forward = direction;
                    break;
                case LookAtDir.Right:
                    thisTransform.right = direction;
                    break;
                case LookAtDir.Up:
                    thisTransform.up = direction;
                    break;
            }

            switch (lookAtScaleCompare)
            {
                case LookAtScale.X:
                    thisTransform.localScale = targetTransform.position.x >= thisTransform.position.x
                        ? lookAtDefaultScale
                        : lookAtCompareScale;
                    break;
                case LookAtScale.Y:
                    thisTransform.localScale = targetTransform.position.y >= thisTransform.position.y
                        ? lookAtDefaultScale
                        : lookAtCompareScale;
                    break;
                case LookAtScale.Z:
                    thisTransform.localScale = targetTransform.position.z >= thisTransform.position.z
                        ? lookAtDefaultScale
                        : lookAtCompareScale;
                    break;
            }
        }
    }
}