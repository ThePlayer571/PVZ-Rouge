using Cinemachine;
using QFramework;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Others.LevelScene
{
    public class VirtualCameraController : MonoBehaviour, IController
    {
        [SerializeField] private float offsetScaleX = 10f;
        [SerializeField] private float offsetScaleY = 6f;
        private PlayerInputControl _inputActions;
        private CinemachineVirtualCamera _virtualCamera;
        private CinemachineFramingTransposer _transposer;

        private void Awake()
        {
            _virtualCamera = GetComponent<CinemachineVirtualCamera>();
            if (_virtualCamera == null)
            {
                Debug.LogError("CinemachineVirtualCamera not found!");
                enabled = false;
                return;
            }

            _transposer = _virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            _inputActions = new PlayerInputControl();
        }

        private void OnEnable()
        {
            _inputActions.Enable();
        }

        private void OnDisable()
        {
            _inputActions.Disable();
        }

        private void Update()
        {
            Vector2 offset = _inputActions.Level.CamaraOffset.ReadValue<Vector2>();
            if (offset.sqrMagnitude > 0.0001f) // 有输入
            {
                _transposer.m_TrackedObjectOffset = new Vector3(offset.x * offsetScaleX, offset.y * offsetScaleY, 0);
            }
            else // 没有输入
            {
                _transposer.m_TrackedObjectOffset = Vector3.zero;
            }
        }

        public IArchitecture GetArchitecture()
        {
            return PVZRouge.Interface;
        }
    }
}