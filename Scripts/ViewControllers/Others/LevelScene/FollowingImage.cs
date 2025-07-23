using QAssetBundle;
using QFramework;
using TPL.PVZR.Systems.Level_Data;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace TPL.PVZR.ViewControllers.Others.LevelScene
{
    public class FollowingImage : MonoBehaviour, IController
    {
        private Image _Image;
        private Sprite ShovelSprite;

        private IHandSystem _HandSystem;

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

            resLoader.Recycle2Cache();

            _HandSystem = this.GetSystem<IHandSystem>();
            _HandSystem.HandInfo.RegisterWithInitValue(val =>
            {
                switch (val.HandState)
                {
                    case HandState.Empty:
                        _following = false;
                        _Image.sprite = null;
                        _Image.enabled = false;
                        transform.localScale = Vector3.one;
                        break;
                    case HandState.HaveSeed:
                        _following = true;
                        _Image.enabled = true;
                        _Image.sprite = val.PickedSeed.CardData.CardDefinition.FollowingSprite;
                        _Image.SetNativeSize();
                        break;
                    case HandState.HaveShovel:
                        _following = true;
                        _Image.sprite = ShovelSprite;
                        _Image.enabled = true;
                        transform.localScale = Vector3.one * 1.53f;
                        break;
                }
            }).UnRegisterWhenGameObjectDestroyed(this);
        }

        public IArchitecture GetArchitecture()
        {
            return PVZRouge.Interface;
        }
    }
}