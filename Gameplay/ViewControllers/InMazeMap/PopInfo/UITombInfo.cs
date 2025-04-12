using System;
using DG.Tweening;
using QFramework;
using TPL.PVZR.Architecture.Models;
using UnityEngine;
using UnityEngine.UI;
using QAssetBundle;
using TMPro;


namespace TPL.PVZR.Gameplay.ViewControllers.InMazeMap.PopInfo
{
    public class UITombInfo: UISpotInfo
    {
        
        [SerializeField] private TextMeshProUGUI TextTemp;
        [SerializeField] private Image Bg;
         private float moveDistance = 0.5f;
        
        # region 公有
        public override void Show()
        {
            Bg.DOFade(1, 0.5f).SetEase(Ease.OutQuad);
            Bg.rectTransform.DOAnchorPosY(Bg.rectTransform.anchoredPosition.y + moveDistance, 0.5f).SetEase(Ease.OutQuad);
            TextTemp.DOFade(1, 0.5f).SetEase(Ease.OutQuad);
            TextTemp.rectTransform.DOAnchorPosY(TextTemp.rectTransform.anchoredPosition.y + moveDistance, 0.5f).SetEase(Ease.OutQuad);
        }

        public override void HideInstant()
        {
            Bg.DOFade(0, 0).SetEase(Ease.OutQuad);
            Bg.rectTransform.DOAnchorPosY(Bg.rectTransform.anchoredPosition.y- moveDistance, 0).SetEase(Ease.OutQuad);
            TextTemp.DOFade(0, 0).SetEase(Ease.OutQuad);
            TextTemp.rectTransform.DOAnchorPosY(TextTemp.rectTransform.anchoredPosition.y - moveDistance, 0).SetEase(Ease.OutQuad);
        }
        
        public override void HideAndDestroy()
        {
            ActionKit.Sequence()
                .Callback(() =>
                {
                    Bg.DOFade(0, 0.5f).SetEase(Ease.OutQuad);
                    Bg.rectTransform.DOAnchorPosY(Bg.rectTransform.anchoredPosition.y - moveDistance, 0.5f).SetEase(Ease.OutQuad);
                    TextTemp.DOFade(0, 0.5f).SetEase(Ease.OutQuad);
                    TextTemp.rectTransform.DOAnchorPosY(TextTemp.rectTransform.anchoredPosition.y - moveDistance, 0.5f).SetEase(Ease.OutQuad);
                })
                .Delay(0.51f)
                .Callback(() =>
                {
                    gameObject.DestroySelf();
                }).Start(this);
        }
        # endregion
        
        # region 私有

        private static readonly ResLoader ResLoader = ResLoader.Allocate();

        private void Awake()
        {
            Bg = GetComponent<Image>();
        }

        #endregion
        # region Create

        public static UITombInfo Create(TombData data)
        {
            // 创建与初始化
            UITombInfo go = ResLoader.LoadSync<GameObject>(Mazemap_uitombinfo_prefab.BundleName, Mazemap_uitombinfo_prefab.MazeMap_UITombInfo)
                .Instantiate().GetComponent<UITombInfo>();
            // 初始化设置
            go.transform.position = data.WorldPosition + new Vector3(0, 1.6f, 0);
            
            //
            go.transform.SetParent(ReferenceModel.Get.WorldSpaceCanvas);
            
            // 不显示
            go.HideInstant();
            //
            return go;
        }

        #endregion
    }
}