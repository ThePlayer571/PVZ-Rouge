using System;
using DG.Tweening;
using QFramework;
using TPL.PVZR.CommandEvents.__NewlyAdded__;
using TPL.PVZR.CommandEvents.Level_Gameplay;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TPL.PVZR.ViewControllers
{
    public sealed class Sun : MonoBehaviour, IController, IPointerEnterHandler
    {
        private SpriteRenderer _SpriteRenderer;
        private bool _isCollected = false;
        [SerializeField] public int SunPoint = 25;


        private void Awake()
        {
            _SpriteRenderer = this.GetComponent<SpriteRenderer>();
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
            this.SendCommand<CollectSunCommand>(new CollectSunCommand(this));
        }


        public IArchitecture GetArchitecture()
        {
            return PVZRouge.Interface;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            TryCollect();
        }
    }
}