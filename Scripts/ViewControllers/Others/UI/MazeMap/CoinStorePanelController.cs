using System;
using System.Collections.Generic;
using QFramework;
using TPL.PVZR.CommandEvents.__NewlyAdded__;
using TPL.PVZR.Helpers;
using TPL.PVZR.Models;
using TPL.PVZR.Systems;
using UnityEngine;
using UnityEngine.UI;

namespace TPL.PVZR.ViewControllers.Others.UI.MazeMap
{
    public class CoinStorePanelController : MonoBehaviour, IController
    {
        [SerializeField] private Toggle mainToggle;
        [SerializeField] private Toggle toggle;
        [SerializeField] private RectTransform View;

        [SerializeField] private RectTransform Trades;
        [SerializeField] private GameObject TradePrefab;

        private ICoinStoreSystem _CoinStoreSystem;
        private IGameModel _GameModel;
        private List<CoinTradeNode> TradeList = new();

        private void Awake()
        {
            _CoinStoreSystem = this.GetSystem<ICoinStoreSystem>();
            _GameModel = this.GetModel<IGameModel>();
        }

        private void Start()
        {
            toggle.onValueChanged.AddListener(Display);
            mainToggle.onValueChanged.AddListener(Display);

            for (int index = 0; index < 10; index++)
            {
                var tradeData = _CoinStoreSystem.GetCoinTradeByIndex(index);
                // 创建Trade节点
                var trade = TradePrefab.Instantiate().GetComponent<CoinTradeNode>();
                trade.transform.SetParent(Trades, false);
                trade.Show();
                TradeList.Add(trade);

                // 设置TradeUI
                trade.CoinText.text = tradeData.CoinAmount.ToString();
                var lootView = ItemViewFactory.CreateItemView(tradeData.LootData);
                lootView.transform.SetParent(trade.ForSale, false);

                // 初始化UI
                _GameModel.GameData.InventoryData.Coins.RegisterWithInitValue(val =>
                {
                    trade.TradeBtn.interactable = val >= tradeData.CoinAmount && !tradeData.Used;
                }).UnRegisterWhenGameObjectDestroyed(this);
                
                // == 交易事件
                var capturedIndex = index;
                trade.TradeBtn.onClick.AddListener(() =>
                    {
                        this.SendCommand<CoinTradeCommand>(new CoinTradeCommand(capturedIndex));
                        
                        // UI改变
                        lootView.Hide();
                    }
                );
            }
        }

        private void OnDestroy()
        {
            toggle.onValueChanged.RemoveListener(Display);
            mainToggle.onValueChanged.RemoveListener(Display);
            
            // 清理TradeList
            foreach (var trade in TradeList)
            {
                trade.TradeBtn.onClick.RemoveAllListeners();
            }
        }

        private void Display(bool val)
        {
            if (mainToggle.isOn && toggle.isOn) View.Show();
            else View.Hide();
        }


        public IArchitecture GetArchitecture()
        {
            return PVZRouge.Interface;
        }
    }
}