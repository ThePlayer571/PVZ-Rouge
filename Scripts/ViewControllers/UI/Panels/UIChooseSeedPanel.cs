using System;
using System.Collections.Generic;
using DG.Tweening;
using QFramework;
using TPL.PVZR.CommandEvents.Level_ChooseSeeds;
using TPL.PVZR.Helpers.New.GameObjectFactory;
using TPL.PVZR.Models;
using TPL.PVZR.Services;
using TPL.PVZR.ViewControllers.Others.UI;
using UnityEngine;
using UnityEngine.UI;

namespace TPL.PVZR.ViewControllers.UI
{
    public class UIChooseSeedPanelData : UIPanelData
    {
    }

    // 这个UIPanel干的事情有点多，是“需要在ReferenceHelper里获取”的程度
    // 和几个Command和System都有关联
    public partial class UIChooseSeedPanel : UIPanel, IController
    {
        private IPhaseModel _PhaseModel;
        private IGameModel _GameModel;

        [NonSerialized] public List<SeedOptionController> chosenSeedOptions = new();
        [NonSerialized] public List<SeedOptionController> seedOptions = new();

        protected override void OnInit(IUIData uiData = null)
        {
            mData = uiData as UIChooseSeedPanelData ?? new UIChooseSeedPanelData();
            // please add init code here
            _PhaseModel = this.GetModel<IPhaseModel>();
            _GameModel = this.GetModel<IGameModel>();

            ViewTheMapBtn.onClick.AddListener(() =>
            {
                if (_PhaseModel.GamePhase != GamePhase.ChooseSeeds) return;
                if (_isUIVisible) HideUI();
                else ShowUI();
            });
            StartGameBtn.onClick.AddListener(() =>
            {
                if (_PhaseModel.GamePhase != GamePhase.ChooseSeeds) return;
                this.SendCommand<OnStartGameBtnPressedCommand>();
            });
        }

        private bool _isUIVisible = true; // UI可见性（不包含ViewTheMapBtn）

        #region DisplayUI

        private void HideUI()
        {
            if (!_isUIVisible) return;
            _isUIVisible = false;

            //
            InventorySeeds.DOAnchorPosX(-900, 0.2f);
            ChosenSeeds.DOAnchorPosX(-900, 0.2f);
            (StartGameBtn.transform as RectTransform).DOAnchorPosX(2060, 0.2f);
        }

        public void HideAllUI()
        {
            HideUI();
            (ViewTheMapBtn.transform as RectTransform).DOAnchorPosX(2060, 0.2f);
        }

        private void ShowUI()
        {
            if (_isUIVisible) return;
            _isUIVisible = true;

            //
            InventorySeeds.DOAnchorPosX(0, 0.2f);
            ChosenSeeds.DOAnchorPosX(0, 0.2f);
            (StartGameBtn.transform as RectTransform).DOAnchorPosX(1700, 0.2f);
        }

        #endregion

        protected override void OnOpen(IUIData uiData = null)
        {
            chosenSeedOptions = new List<SeedOptionController>();

            // 设置ChosenSeeds长度
            var GridLayoutGroup = ChosenSeeds.GetComponent<GridLayoutGroup>();
            var PaddingLeft = GridLayoutGroup.padding.left;
            var CellSizeX = GridLayoutGroup.cellSize.x;
            var SpacingX = GridLayoutGroup.spacing.x;
            var SeedSlotCount = _GameModel.GameData.InventoryData.SeedSlotCount.Value;
            var width = 2 * PaddingLeft + (CellSizeX + SpacingX) * SeedSlotCount - SpacingX;
            ChosenSeeds.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);

            // 生成待选卡区域的卡牌
            var cardDataList = _GameModel.GameData.InventoryData.Cards;
            foreach (var cardData in cardDataList)
            {
                var go = ShitFactory.CreateSeedOptionController(cardData);
                go.transform.SetParent(InventorySeeds, false);
                seedOptions.Add(go);
            }
        }


        protected override void OnClose()
        {
            chosenSeedOptions = null;
        }

        protected override void OnShow()
        {
        }

        protected override void OnHide()
        {
        }

        public IArchitecture GetArchitecture()
        {
            return PVZRouge.Interface;
        }
    }
}