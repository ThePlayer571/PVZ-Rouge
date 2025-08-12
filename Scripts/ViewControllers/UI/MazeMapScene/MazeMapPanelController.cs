using DG.Tweening;
using QFramework;
using TMPro;
using TPL.PVZR.Classes.DataClasses.Tomb;
using TPL.PVZR.CommandEvents.__NewlyAdded__;
using TPL.PVZR.CommandEvents._NotClassified_;
using TPL.PVZR.Services;
using UnityEngine;
using UnityEngine.UI;

namespace TPL.PVZR.ViewControllers.Others.UI.MazeMap
{
    public class MazeMapPanelController : MonoBehaviour, IController, ICanPutInUIStack
    {
        // 引用
        [SerializeField] private Toggle toggle;
        [SerializeField] private RectTransform View;

        [SerializeField] private CanvasGroup CanvasGroup;

        [SerializeField] private RectTransform PreviewPanelBg;
        [SerializeField] private TextMeshProUGUI LevelNameText;
        [SerializeField] private Button StartLevelBtn;

        [SerializeField] private Button BlackMaskBtn;

        private IUIStackService _UIStackService;

        // UIInfo
        private ITombData _openedTombData;

        private void Start()
        {
            _UIStackService = this.GetService<IUIStackService>();
            HideUIInstantly();

            this.RegisterEvent<OpenLevelPreviewPanelEvent>(e =>
            {
                SetUIInfo(e.Tomb, e.Interactable);
                ShowUI();
            }).UnRegisterWhenGameObjectDestroyed(this);

            BlackMaskBtn.onClick.AddListener(HideUI);

            StartLevelBtn.onClick.AddListener(() =>
            {
                this.GetService<IGamePhaseChangeService>().StartLevel(_openedTombData);
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
            _UIStackService.PushPanel(this);
            CanvasGroup.blocksRaycasts = true;
            CanvasGroup.DOFade(1, 0.2f).SetUpdate(true);
            PreviewPanelBg.DOAnchorPosY(0, 0.2f).SetUpdate(true);
        }

        private void HideUI()
        {
            _UIStackService.PopIfTop(this);
            CanvasGroup.blocksRaycasts = false;
            CanvasGroup.DOFade(0, 0.2f).SetUpdate(true);
            PreviewPanelBg.DOAnchorPosY(-80, 0.2f).SetUpdate(true);
        }

        private void HideUIInstantly()
        {
            _UIStackService.PopIfTop(this);
            CanvasGroup.blocksRaycasts = false;
            CanvasGroup.alpha = 0;
            PreviewPanelBg.anchoredPosition = new Vector2(PreviewPanelBg.anchoredPosition.x, -80);
        }


        private void Display(bool show)
        {
            if (show) View.Show();
            else
            {
                HideUIInstantly();
                View.Hide();
            }
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

        void ICanPutInUIStack.OnPopped()
        {
            HideUI();
        }
    }
}