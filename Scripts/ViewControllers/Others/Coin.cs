using DG.Tweening;
using QFramework;
using TPL.PVZR.CommandEvents.Level_Gameplay.Coins;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TPL.PVZR.ViewControllers
{
    public sealed class Coin : MonoBehaviour, IController, IPointerEnterHandler
    {
        private SpriteRenderer _SpriteRenderer;
        private bool _isCollected = false;

        public int value;


        private void Awake()
        {
            _SpriteRenderer = this.GetComponent<SpriteRenderer>();
        }

        public void Initialize(Sprite sprite, int value)
        {
            _SpriteRenderer.sprite = sprite;
            this.value = value;
        }

        public void OnCollected()
        {
            _isCollected = true;
            transform.DOMoveY(transform.position.y + 0.3f, 0.4f).SetEase(Ease.OutQuad);
            _SpriteRenderer.DOFade(0, 0.4f).OnComplete(() =>
            {
                DOTween.Kill(transform);
                DOTween.Kill(_SpriteRenderer);
                gameObject.DestroySelf();
            });
        }


        public void TryCollect()
        {
            if (_isCollected) return; // 如果已经被收集了，就不再执行收集逻辑
            this.SendCommand<CollectCoinCommand>(new CollectCoinCommand(this));
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            TryCollect();
        }

        public IArchitecture GetArchitecture()
        {
            return PVZRouge.Interface;
        }
    }
}