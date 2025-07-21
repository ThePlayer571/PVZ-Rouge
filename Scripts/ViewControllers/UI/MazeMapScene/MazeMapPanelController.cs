using System;
using DG.Tweening;
using QFramework;
using TMPro;
using TPL.PVZR.Classes.DataClasses.Tomb;
using TPL.PVZR.CommandEvents.__NewlyAdded__;
using TPL.PVZR.CommandEvents._NotClassified_;
using UnityEngine;
using UnityEngine.UI;

namespace TPL.PVZR.ViewControllers.Others.UI.MazeMap
{
    public class MazeMapPanelController : MonoBehaviour, IController
    {
        // 引用
        [SerializeField] private Toggle toggle;
        [SerializeField] private RectTransform View;

        [SerializeField] private CanvasGroup CanvasGroup;

        [SerializeField] private RectTransform PreviewPanelBg;
        [SerializeField] private TextMeshProUGUI LevelNameText;
        [SerializeField] private Button StartLevelBtn;

        [SerializeField] private Button BlackMaskBtn;

        // UIInfo
        private ITombData _openedTombData;

        private void Start()
        {
            HideUIInstantly();

            this.RegisterEvent<OpenLevelPreviewPanelEvent>(e =>
            {
                SetUIInfo(e.Tomb, e.Interactable);
                ShowUI();
            }).UnRegisterWhenGameObjectDestroyed(this);

            BlackMaskBtn.onClick.AddListener(HideUI);

            StartLevelBtn.onClick.AddListener(() =>
            {
                this.SendCommand<StartLevelCommand>(new StartLevelCommand(_openedTombData));
            });

            toggle.onValueChanged.AddListener(Display);
        }

        private void SetUIInfo(ITombData tombData, bool interactable)
        {
            _openedTombData = tombData;
            LevelNameText.text = tombData.LevelDefinition.LevelDef.Id.ToString();
            StartLevelBtn.interactable = interactable;
        }

        private void ShowUI()
        {
            CanvasGroup.blocksRaycasts = true;
            CanvasGroup.DOFade(1, 0.2f);
            PreviewPanelBg.DOAnchorPosY(0, 0.2f);
        }

        private void HideUI()
        {
            CanvasGroup.blocksRaycasts = false;
            CanvasGroup.DOFade(0, 0.2f);
            PreviewPanelBg.DOAnchorPosY(-80, 0.2f);
        }

        private void HideUIInstantly()
        {
            CanvasGroup.blocksRaycasts = false;
            CanvasGroup.alpha = 0;
            PreviewPanelBg.anchoredPosition = new Vector2(PreviewPanelBg.anchoredPosition.x, -80);
        }


        private void Display(bool show)
        {
            if (show) View.Show();
            else View.Hide();
        }

        private void OnDestroy()
        {
            toggle.onValueChanged.RemoveListener(Display);
            BlackMaskBtn.onClick.RemoveListener(HideUI);
            StartLevelBtn.onClick.RemoveAllListeners();
        }

        public IArchitecture GetArchitecture()
        {
            return PVZRouge.Interface;
        }
    }
}