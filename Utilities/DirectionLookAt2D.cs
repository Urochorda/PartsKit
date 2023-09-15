using UnityEngine;

namespace PartsKit
{
    public class DirectionLookAt2D : MonoBehaviour
    {
        enum FlipTrueDirType
        {
            Forward,
            Right,
            Up,
        }

        enum RotationDir
        {
            Forward,
            Right,
            Up
        }

        enum RotationAxis
        {
            Forward,
            Right,
            Up,
        }

        public enum UpdateType
        {
            Null,
            Update,
            FixUpdate,
        }

        [Header("Common")] [SerializeField] private CheckNullProperty<Transform> target;

        [field: Tooltip("更新方式，选择Null则不会自动更新，只会再调用LookAt时更新一下")]
        [field: SerializeField]
        public UpdateType AutoUpdateType { get; set; } = UpdateType.Null;

        [SerializeField] private RotationDir rotationDirType = RotationDir.Right;
        [SerializeField] private RotationAxis rotationAxisType = RotationAxis.Forward;

        [Header("Rotation")] [SerializeField] private bool isUseRotation;
        [SerializeField] private float rotationSpeed = -1;

        [Header("Rotation Flip")] [SerializeField]
        private bool isUseRotationFlip;

        [SerializeField] private FlipTrueDirType rotationFlipForward = FlipTrueDirType.Right;
        [SerializeField] private float rotationFlipPrecision = 0.001f;
        [SerializeField] private Vector3 rotationFlipDefaultEuler = Vector3.zero;
        [SerializeField] private Vector3 rotationFlipFlipEuler = new Vector3(0, 180, 0);

        [Header("Scale Flip")] [SerializeField]
        private bool isUseScaleFlip;

        [SerializeField] private FlipTrueDirType scaleFlipForward = FlipTrueDirType.Right;
        [SerializeField] private FlipTrueDirType scaleFlipAxis = FlipTrueDirType.Forward;
        [SerializeField] private float scaleFlipPrecision = 0.001f;
        [SerializeField] private Vector3 scaleFlipDefaultScale = Vector3.one;
        [SerializeField] private Vector3 scaleFlipFlipScale = new Vector3(1, -1, 1);

        private Vector3 TargetDirection { get; set; }

        public void SetTarget(Transform tTransform)
        {
            target.SetData(tTransform);
        }

        public void LookAtDir(Vector3 direction)
        {
            TargetDirection = direction.normalized;
            UpdateFlip();
        }

        public void LookAtPoint(Vector3 point)
        {
            if (!target.GetValue(out Transform targetTransform))
            {
                return;
            }

            LookAtDir(point - targetTransform.position);
        }

        private void Update()
        {
            DoUpdateByType(UpdateType.Update);
        }

        private void FixedUpdate()
        {
            DoUpdateByType(UpdateType.FixUpdate);
        }

        private void DoUpdateByType(UpdateType type)
        {
            if (type != AutoUpdateType)
            {
                return;
            }

            UpdateFlip();
        }

        private void UpdateFlip()
        {
            if (!target.GetValue(out Transform targetTransform))
            {
                return;
            }

            if (isUseRotationFlip)
            {
                bool isFlip = CheckShouldFlip(TargetDirection, rotationFlipForward, rotationFlipPrecision,
                    out bool isChange);
                if (isChange)
                {
                    targetTransform.eulerAngles = isFlip ? rotationFlipFlipEuler : rotationFlipDefaultEuler;
                }
            }

            if (isUseRotation)
            {
                Vector3 rotationDir = GetRotationDir(targetTransform);
                Vector3 rotationAxis = GetRotationAxis(targetTransform);
                Vector3 targetDirValue = Vector3.ProjectOnPlane(TargetDirection, rotationAxis).normalized;
                targetTransform.rotation = GetRotationDir(targetTransform, rotationDir, targetDirValue, rotationAxis);
            }

            if (isUseScaleFlip)
            {
                Vector3 rotationForward = GetRotationDir(targetTransform);
                Vector3 rotationAxis = GetRotationAxis(targetTransform);
                targetTransform.localScale =
                    CheckShouldFlip(rotationForward, scaleFlipForward, scaleFlipPrecision, out _) ==
                    CheckShouldFlip(rotationAxis, scaleFlipAxis, scaleFlipPrecision, out _)
                        ? scaleFlipDefaultScale
                        : scaleFlipFlipScale; //缩放是实时的
            }
        }

        private bool CheckShouldFlip(Vector3 targetDir, FlipTrueDirType lookAtScaleType, float precision,
            out bool isChange)
        {
            float targetValue = 0;
            switch (lookAtScaleType)
            {
                case FlipTrueDirType.Forward:
                    targetValue = targetDir.z;
                    break;
                case FlipTrueDirType.Right:
                    targetValue = targetDir.x;
                    break;
                case FlipTrueDirType.Up:
                    targetValue = targetDir.y;
                    break;
            }

            isChange = targetValue > precision || targetValue < -precision;
            return targetValue < precision;
        }

        private Vector3 GetRotationDir(Transform targetTransform)
        {
            Vector3 rotationDir = targetTransform.forward;
            switch (rotationDirType)
            {
                case RotationDir.Forward:
                    rotationDir = targetTransform.forward;
                    break;
                case RotationDir.Right:
                    rotationDir = targetTransform.right;
                    break;
                case RotationDir.Up:
                    rotationDir = targetTransform.up;
                    break;
            }

            return rotationDir;
        }

        private Vector3 GetRotationAxis(Transform targetTransform)
        {
            Vector3 axis = Vector3.forward;
            switch (rotationAxisType)
            {
                case RotationAxis.Forward:
                    axis = targetTransform.forward;
                    break;
                case RotationAxis.Right:
                    axis = targetTransform.right;
                    break;
                case RotationAxis.Up:
                    axis = targetTransform.up;
                    break;
            }

            return axis;
        }


        private Quaternion GetRotationDir(Transform targetTransform, Vector3 curDir, Vector3 targetDir, Vector3 axis)
        {
            float angle = Vector3.SignedAngle(curDir, targetDir, axis);

            if (rotationSpeed > 0)
            {
                bool isNegative = angle < 0;
                angle = Mathf.Abs(angle);
                angle = Mathf.Lerp(0, angle, rotationSpeed / angle * Time.deltaTime);
                if (isNegative)
                {
                    angle *= -1;
                }
            }

            return Quaternion.AngleAxis(angle, axis) * targetTransform.rotation;
        }
    }
}