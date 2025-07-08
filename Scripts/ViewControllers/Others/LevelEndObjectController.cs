using System;
using DG.Tweening;
using QFramework;
using TPL.PVZR.CommandEvents.__NewlyAdded__;
using TPL.PVZR.Tools;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Others
{
    public class LevelEndObjectController : MonoBehaviour, IController
    {
        [SerializeField] private InteractionPromptController _InteractionPromptController;
        [SerializeField] private CollisionDetector _CollisionDetector;
        private PlayerInputControl _inputActions;

        private void Awake()
        {
            _inputActions = new PlayerInputControl();

            _inputActions.Level.Enable();
            _inputActions.Level.InteractionE.performed += context =>
            {
                if (_CollisionDetector.HasTarget)
                {
                    this.SendCommand<OnCollectLevelEndObjectCommand>();
                }
            };

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

        private void OnDestroy()
        {
            _inputActions.Level.Disable();
        }

        private void Update()
        {
            var targetPos = ReferenceHelper.Player.transform.position;

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