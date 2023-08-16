using Cinemachine;
using UnityEngine;

namespace PartsKit
{
    public class CinemachineCameraItem : MonoBehaviour
    {
        [SerializeField] private string cameraName;
        [SerializeField] private bool defaultActive;
        private bool isRegister;

        public ICinemachineCamera CinemachineCamera { get; private set; }
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
            CinemachineCamera = GetComponent<ICinemachineCamera>();
            if (CinemachineCamera != null)
            {
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