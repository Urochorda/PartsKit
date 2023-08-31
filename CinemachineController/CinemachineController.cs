using System.Collections.Generic;
using System.Linq;
using Cinemachine;

namespace PartsKit
{
    public class CinemachineController
    {
        private const int UnActivePriority = 0;
        private const int ActivePriority = 1;
        private static CinemachineController instance;

        public static CinemachineController Instance => instance ??= new CinemachineController();

        private readonly HashSet<CinemachineCameraItem>
            cinemachineCameras = new HashSet<CinemachineCameraItem>();

        /// <summary>
        /// 注册相机
        /// </summary>
        /// <param name="camera"></param>
        public void RegisterCamera(CinemachineCameraItem camera)
        {
            cinemachineCameras.Add(camera);
            SetCameraItemActiveState(camera.CameraName, camera.DefaultActive);
        }

        /// <summary>
        /// 取消相机注册
        /// </summary>
        /// <param name="camera"></param>
        public void UnRegisterCamera(CinemachineCameraItem camera)
        {
            cinemachineCameras.Remove(camera);
        }

        /// <summary>
        /// 获取相机对象
        /// </summary>
        /// <param name="name"></param>
        /// <param name="cameraItem"></param>
        /// <returns></returns>
        public bool GetCameraItem(string name, out CinemachineCameraItem cameraItem)
        {
            cameraItem = cinemachineCameras.First(item => item.CameraName == name);
            return cameraItem != null;
        }

        /// <summary>
        /// 获取激活的相机
        /// </summary>
        /// <param name="cameraItem"></param>
        /// <param name="brainIndex"></param>
        /// <returns></returns>
        public bool GetActiveCameraItem(out CinemachineCameraItem cameraItem, int brainIndex = 0)
        {
            CinemachineBrain brain = CinemachineCore.Instance.GetActiveBrain(brainIndex);
            if (brain == null || brain.ActiveVirtualCamera == null ||
                !brain.ActiveVirtualCamera.VirtualCameraGameObject.TryGetComponent(out cameraItem))
            {
                cameraItem = null;
                return false;
            }

            return true;
        }

        /// <summary>
        /// 设置激活相机状态，激活相机时会自动禁止上一个激活的相机
        /// </summary>
        public void SetActiveCameraItem(string name)
        {
            SetCameraItemActiveState(name, true);
        }

        private void SetCameraItemActiveState(string name, bool isActive)
        {
            if (!GetCameraItem(name, out CinemachineCameraItem curCameraItem))
            {
                return;
            }

            if (isActive && GetActiveCameraItem(out CinemachineCameraItem lastCameraItem))
            {
                SetCameraPriorityByActive(lastCameraItem, false);
            }

            SetCameraPriorityByActive(curCameraItem, isActive);
        }

        private void SetCameraPriorityByActive(CinemachineCameraItem lastCameraItem, bool isActive)
        {
            if (isActive)
            {
                CinemachineCameraItem.SetPriorityValue(lastCameraItem, ActivePriority);
                lastCameraItem.gameObject.SetActive(true);
            }
            else
            {
                CinemachineCameraItem.SetPriorityValue(lastCameraItem, UnActivePriority);
            }
        }
    }
}