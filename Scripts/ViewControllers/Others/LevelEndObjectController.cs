using System;
using QFramework;
using TPL.PVZR.CommandEvents._NotClassified_;
using TPL.PVZR.Tools;
using TPL.PVZR.ViewControllers.Managers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TPL.PVZR.ViewControllers.Others
{
    public class LevelEndObjectController : MonoBehaviour, IController
    {
        [SerializeField] private InteractionPromptController _InteractionPromptController;
        [SerializeField] private TriggerDetector _CollisionDetector;


        private void Awake()
        {
            InputManager.Instance.InputActions.Level.InteractionE.performed += OnInterationEPressed;

            _CollisionDetector.OnTargetCountChanged.Register(count =>
            {
                if (count > 0)
                {
                    _InteractionPromptController.ShowPrompt();
                }
                else
                {
                    _InteractionPromptController.HidePrompt();
                }
            }).UnRegisterWhenGameObjectDestroyed(this);
        }

        private void OnInterationEPressed(InputAction.CallbackContext _)
        {
            if (_CollisionDetector.HasTarget)
            {
                this.SendCommand<OnCollectLevelEndObjectCommand>();
            }
        }

        private void OnDestroy()
        {
            InputManager.Instance.InputActions.Level.InteractionE.performed -= OnInterationEPressed;
        }

        private void Update()
        {
            var targetPos = Player.Instance.transform.position;

            // 计算距离
            var distance = Vector3.Distance(transform.position, targetPos);

            // 根据距离调整速度
            var speed = Mathf.Clamp(distance, 1f, 10f);

            // 移动到目标位置
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
        }


        private void OnTriggerEnter2D(Collider2D other)
        {
            _InteractionPromptController.ShowPrompt();
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            _InteractionPromptController.HidePrompt();
        }

        public IArchitecture GetArchitecture()
        {
            return PVZRouge.Interface;
        }
    }
}