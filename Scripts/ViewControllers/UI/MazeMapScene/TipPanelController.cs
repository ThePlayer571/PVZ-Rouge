using System;
using DG.Tweening;
using QFramework;
using TMPro;
using TPL.PVZR.Classes.Others;
using TPL.PVZR.Services;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace TPL.PVZR.ViewControllers.Others.UI.MazeMap
{
    public class TipPanelController : MonoBehaviour, IController, ICanPutInUIStack
    {
        // SideBar
        [SerializeField] private RectTransform SideBar;
        [SerializeField] private GameObject TipPrefab;

        // PopUI
        [SerializeField] private RectTransform View;
        [SerializeField] private CanvasGroup CanvasGroup;
        [SerializeField] private RectTransform Bg;
        [SerializeField] private Button CloseBtn;

        [SerializeField] private TextMeshProUGUI TitleText;
        [SerializeField] private TextMeshProUGUI BodyText;

        //
        private ITipService _TipService;
        private IUIStackService _UIStackService;

        private void Awake()
        {
            _TipService = this.GetService<ITipService>();
            _UIStackService = this.GetService<IUIStackService>();

            // 初始化SideBar
            while (_TipService.HasTip())
            {
                var tip = _TipService.PopTip();
                var go = TipPrefab.Instantiate();
                go.transform.SetParent(SideBar, false);
                go.Show();
                //todo 做成Component+Init的形式
                var btn = go.GetComponent<Button>();
                btn.onClick.AddListener(() =>
                {
                    go.DestroySelf();
                    SetPopUIInfo(tip);
                    ShowPopUI();
                });
            }

            // 初始化PopUI
            HidePopUIInstantly();
            View.Show();

            CloseBtn.onClick.AddListener(HidePopUI);
        }

        #region PopUI

        void ICanPutInUIStack.OnPopped()
        {
            HidePopUI();
        }

        #region Display

        private void SetPopUIInfo(TipDefinition tip)
        {
            TitleText.text = tip.Title;
            BodyText.text = tip.Body;
        }

        private void ShowPopUI()
        {
            _UIStackService.PushPanel(this);
            CanvasGroup.blocksRaycasts = true;
            CanvasGroup.DOFade(1, 0.2f).SetUpdate(true);
            Bg.DOAnchorPosY(0, 0.2f).SetUpdate(true);
        }

        private void HidePopUI()
        {
            _UIStackService.PopIfTop(this);
            CanvasGroup.blocksRaycasts = false;
            CanvasGroup.DOFade(0, 0.2f).SetUpdate(true);
            Bg.DOAnchorPosY(-80, 0.2f).SetUpdate(true);
        }

        private void HidePopUIInstantly()
        {
            _UIStackService.PopIfTop(this);
            CanvasGroup.blocksRaycasts = false;
            CanvasGroup.alpha = 0;
            Bg.anchoredPosition = new Vector2(Bg.anchoredPosition.x, -80);
        }

        #endregion

        #endregion

        public IArchitecture GetArchitecture()
        {
            return PVZRouge.Interface;
        }
    }
}