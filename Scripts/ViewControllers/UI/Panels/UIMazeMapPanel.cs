using UnityEngine;
using UnityEngine.UI;
using QFramework;
using TPL.PVZR.Models;
using TPL.PVZR.Systems;
using TPL.PVZR.Systems.MazeMap;

namespace TPL.PVZR.ViewControllers.UI
{
    public class UIMazeMapPanelData : UIPanelData
    {
    }

    public partial class UIMazeMapPanel : UIPanel, IController
    {
        private IAwardSystem _AwardSystem;
        private IGameModel _GameModel;

        protected override void OnInit(IUIData uiData = null)
        {
            mData = uiData as UIMazeMapPanelData ?? new UIMazeMapPanelData();
            // please add init code here

            // 
            _AwardSystem = this.GetSystem<IAwardSystem>();
            _GameModel = this.GetModel<IGameModel>();

            // 初始化面板内容
            if (_AwardSystem.AwardCount > 0)
            {
                ActionKit.DelayFrame(1, () => AwardToggle.isOn = true).Start(this);
            }

            // 金币事件订阅
            _GameModel.GameData.InventoryData.Coins.RegisterWithInitValue(val => { CoinText.text = $"coins: {val}"; })
                .UnRegisterWhenGameObjectDestroyed(this);
        }

        protected override void OnOpen(IUIData uiData = null)
        {
        }

        protected override void OnShow()
        {
        }

        protected override void OnHide()
        {
        }

        protected override void OnClose()
        {
        }

        public IArchitecture GetArchitecture()
        {
            return PVZRouge.Interface;
        }
    }
}