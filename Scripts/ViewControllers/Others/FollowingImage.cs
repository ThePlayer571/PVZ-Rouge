using System;
using QAssetBundle;
using QFramework;
using TPL.PVZR.Events.HandEvents;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace TPL.PVZR.ViewControllers.Others
{
    public class FollowingImage : MonoBehaviour, IController
    {
        private Image _Image;
        private Sprite ShovelSprite;

        private bool _following;

        private void Update()
        {
            if (_following)
            {
                // 用 Input System 获取屏幕坐标
                Vector2 screenPosition = Mouse.current.position.ReadValue();

                // 获取 Canvas
                var parentRect = this.transform.parent as RectTransform;
                var selfRect = this.transform as RectTransform;

                // 屏幕坐标转为父节点局部坐标，并设置
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        parentRect, screenPosition, null, out Vector2 localPoint))
                {
                    selfRect.anchoredPosition = localPoint;
                }
            }
        }

        private void Awake()
        {
            var resLoader = ResLoader.Allocate();
            _Image = this.GetComponent<Image>();
            ShovelSprite = resLoader.LoadSync<Sprite>(Shovel_png.BundleName, Shovel_png.Shovel);

            this.RegisterEvent<SelectSeedEvent>(e =>
            {
                _following = true;
                _Image.enabled = true;
                _Image.sprite = e.SelectedSeedData.CardData.CardDefinition.PlantSprite;
            }).UnRegisterWhenGameObjectDestroyed(this);
            this.RegisterEvent<DeselectEvent>(e =>
            {
                _following = false;
                _Image.sprite = null;
                _Image.enabled = false;
                transform.localScale = Vector3.one;
            }).UnRegisterWhenGameObjectDestroyed(this);
            this.RegisterEvent<SelectShovelEvent>(e =>
            {
                _following = true;
                _Image.sprite = ShovelSprite;
                _Image.enabled = true;
                transform.localScale = Vector3.one * 1.53f;
            }).UnRegisterWhenGameObjectDestroyed(this);
        }

        public IArchitecture GetArchitecture()
        {
            return PVZRouge.Interface;
        }
    }
}