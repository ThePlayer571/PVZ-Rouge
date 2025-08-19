using DG.Tweening;
using QFramework;
using TPL.PVZR.Helpers.New.GameObjectFactory;
using TPL.PVZR.Models;
using TPL.PVZR.ViewControllers.Managers;
using TPL.PVZR.ViewControllers.Others.UI;
using UnityEngine;
using UnityEngine.UI;

namespace TPL.PVZR.ViewControllers.UI
{
    public class UILevelGameplayPanelData : UIPanelData
    {
    }

    public partial class UILevelGameplayPanel : UIPanel, IController
    {
        private ILevelModel _LevelModel;


        private bool _isUIVisible = true;
        private bool _isSlotPanelShowing = true;
        public bool _allowChangeUI { private get; set; } = false;
        [SerializeField] private LevelStateBarPanelController LevelStateBarPanelController;


        #region DisplayUI

        public void HideUIInstantly()
        {
            if (!_isUIVisible) return;
            _isUIVisible = false;

            SlotPanel.DOAnchorPosY(200, 0f);
            StateBarPanel.DOAnchorPosY(-44, 0f);
        }

        public void ShowUI()
        {
            if (_isUIVisible) return;
            _isUIVisible = true;

            SlotPanel.DOAnchorPosY(0, 0.2f);
            StateBarPanel.DOAnchorPosY(44, 0.2f);
        }

        private void ShowUISlotPanel()
        {
            _isSlotPanelShowing = true;
            SlotPanel.DOAnchorPosY(0, 0.2f);
        }

        public void HideUISlotPanel()
        {
            _isSlotPanelShowing = false;
            SlotPanel.DOAnchorPosY(200, 0.2f);
        }

        public void SlotPanelVisibilityToggle()
        {
            if (_isSlotPanelShowing)
            {
                HideUISlotPanel();
            }
            else
            {
                ShowUISlotPanel();
            }
        }

        #endregion

        protected override void OnInit(IUIData uiData = null)
        {
            mData = uiData as UILevelGameplayPanelData ?? new UILevelGameplayPanelData();
            // please add init code here

            _LevelModel = this.GetModel<ILevelModel>();

            InputManager.Instance.InputActions.Level.SlotPanelVisibilityToggle.performed +=
                OnSlotPanelVisibilityTogglePressed;

            FoldPanel.onClick.AddListener(() =>
            {
                if (_allowChangeUI) SlotPanelVisibilityToggle();
            });
        }

        private void OnSlotPanelVisibilityTogglePressed(UnityEngine.InputSystem.InputAction.CallbackContext _)
        {
            if (_allowChangeUI) SlotPanelVisibilityToggle();
        }

        protected override void OnOpen(IUIData uiData = null)
        {
            // 调整SeedSlots的大小
            var GridLayoutGroup = SeedSlots.GetComponent<GridLayoutGroup>();
            var PaddingLeft = GridLayoutGroup.padding.left;
            var CellSizeX = GridLayoutGroup.cellSize.x;
            var SpacingX = GridLayoutGroup.spacing.x;
            var SeedSlotCount = Mathf.Max(1, _LevelModel.ChosenSeeds.Count);
            var width = 2 * PaddingLeft + (CellSizeX + SpacingX) * SeedSlotCount - SpacingX;
            SlotPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);

            // 创建SeedController
            for (int i = 0; i < _LevelModel.ChosenSeeds.Count; i++)
            {
                var seedData = _LevelModel.ChosenSeeds[i];
                var go = ShitFactory.CreateSeedController(seedData);
                go.transform.SetParent(SeedSlots, false);
            }

            // 创建LevelStateBarPanelController
            _ = LevelStateBarPanelController.SpawnFlags();
        }

        protected override void OnShow()
        {
        }

        protected override void OnHide()
        {
        }

        protected override void OnClose()
        {
            InputManager.Instance.InputActions.Level.SlotPanelVisibilityToggle.performed -=
                OnSlotPanelVisibilityTogglePressed;
            FoldPanel.onClick.RemoveAllListeners();
        }

        public IArchitecture GetArchitecture()
        {
            return PVZRouge.Interface;
        }
    }
}