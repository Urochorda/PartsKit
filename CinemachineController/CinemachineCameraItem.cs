using Cinemachine;
using UnityEngine;

namespace PartsKit
{
    public class CinemachineCameraItem : MonoBehaviour
    {
        [SerializeField] private string cameraName;
        [SerializeField] private bool defaultActive;
        private bool isRegister;
        private int priority;

        public CheckNullProperty<ICinemachineCamera> CinemachineCamera { get; private set; }
        public bool DefaultActive => defaultActive;

        public string CameraName
        {
            get => cameraName;
            set => gameObject.name = value;
        }

        /// <summary>
        /// 给Cinemachine系统准备时间
        /// </summary>
        private void Start()
        {
            CameraName = cameraName;
            CinemachineCamera = new CheckNullProperty<ICinemachineCamera>(GetComponent<ICinemachineCamera>(), false);
            if (CinemachineCamera.GetValue(out ICinemachineCamera cinemachineCamera))
            {
                priority = cinemachineCamera.Priority;
                CinemachineController.Instance.RegisterCamera(this);
                isRegister = true;
            }
        }

        private void OnDestroy()
        {
            if (isRegister)
            {
                CinemachineController.Instance.UnRegisterCamera(this);
            }
        }

        private void Update()
        {
            //camera的Priority由cameraItem接管了
            if (CinemachineCamera.GetValue(out ICinemachineCamera cinemachineCamera))
            {
                cinemachineCamera.Priority = priority;
            }
        }

        /// <summary>
        /// 管理器调用，切勿手动调用
        /// </summary>
        /// <param name="value"></param>
        public void SetPriorityValue(int value)
        {
            priority = value;
            if (CinemachineCamera.GetValue(out ICinemachineCamera cinemachineCamera))
            {
                cinemachineCamera.Priority = priority;
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (string.IsNullOrEmpty(cameraName))
            {
                cameraName = gameObject.name;
            }

            CameraName = cameraName;
        }
#endif
    }
}