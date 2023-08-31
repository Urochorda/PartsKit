using Cinemachine;
using UnityEngine;

namespace PartsKit
{
    [RequireComponent(typeof(ICinemachineCamera))]
    public class CinemachineCameraItem : MonoBehaviour
    {
        /// <summary>
        /// 管理器调用，切勿手动调用
        /// </summary>
        public static void SetPriorityValue(CinemachineCameraItem cameraItem, int value)
        {
            cameraItem.priority = value;
            cameraItem.CinemachineCamera.Priority = value;
        }

        [SerializeField] private string cameraName;
        [SerializeField] private bool defaultActive;
        private bool isRegister;
        private int priority;

        public ICinemachineCamera CinemachineCamera { get; private set; }
        public bool DefaultActive => defaultActive;

        public string CameraName
        {
            get => cameraName;
            set
            {
                cameraName = value;
                gameObject.name = value;
            }
        }

        /// <summary>
        /// 给Cinemachine系统准备时间
        /// </summary>
        private void Start()
        {
            CameraName = cameraName;
            CinemachineCamera = GetComponent<ICinemachineCamera>();
            priority = CinemachineCamera.Priority;
            CinemachineController.Instance.RegisterCamera(this);
            isRegister = true;
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
            CinemachineCamera.Priority = priority;
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