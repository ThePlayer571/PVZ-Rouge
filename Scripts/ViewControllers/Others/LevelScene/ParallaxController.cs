using Cinemachine;
using QFramework;
using TPL.PVZR.CommandEvents.New.Level_Shit;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Others.LevelScene
{
    public class ParallaxController : MonoBehaviour, IController
    {
        [Tooltip("Parallax factor, 0=static background, 1=follows camera exactly")] [Range(0f, 1f)]
        public float parallaxFactor = 0.5f;

        [Tooltip("Reference to the Cinemachine Virtual Camera")]
        public CinemachineVirtualCamera virtualCamera;

        private Vector3 _lastCameraPosition;
        private Camera _mainCamera;

        void Start()
        {
            this.RegisterEvent<OnLevelGameObjectPrepared>(e =>
                {
                   InitializeCamera();
                })
                .UnRegisterWhenGameObjectDestroyed(this);
        }

        private void InitializeCamera()
        {
            _mainCamera = Camera.main;

            // 验证摄像机是否正确初始化
            if (virtualCamera == null || _mainCamera == null)
            {
                Debug.LogWarning("No camera found for parallax effect.");
                enabled = false;
            }

            // 检查摄像机rect是否有效
            if (_mainCamera.rect.width <= 0 || _mainCamera.rect.height <= 0)
            {
                Debug.LogWarning("Camera rect is invalid, waiting for proper initialization...");
            }

            _lastCameraPosition = virtualCamera.State.FinalPosition;
        }

        void LateUpdate()
        {
            if (_mainCamera == null) return;
            Vector3 cameraDelta = virtualCamera.State.FinalPosition - _lastCameraPosition;
            transform.position += cameraDelta * parallaxFactor;
            _lastCameraPosition = virtualCamera.State.FinalPosition;
        }

        public IArchitecture GetArchitecture()
        {
            return PVZRouge.Interface;
        }
    }
}