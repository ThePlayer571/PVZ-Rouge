using System.Collections.Generic;
using QFramework;
using TPL.PVZR.CommandEvents.__NewlyAdded__;
using TPL.PVZR.Helpers.New;
using TPL.PVZR.Helpers.New.DataReader;
using TPL.PVZR.Helpers.New.GameObjectFactory;
using TPL.PVZR.Models;
using TPL.PVZR.Systems.MazeMap;
using TPL.PVZR.ViewControllers.Others.UI.ItemView;
using UnityEngine;
using UnityEngine.UI;

namespace TPL.PVZR.ViewControllers.Others.UI.MazeMap
{
    public class SellTradePanelController : MonoBehaviour, IController
    {
        [SerializeField] private Toggle mainToggle;
        [SerializeField] private Toggle toggle;
        [SerializeField] private RectTransform View;

        [SerializeField] private RectTransform Trades;
        [SerializeField] private GameObject TradePrefab;

        private ISellStoreSystem _SellStoreSystem;
        private IGameModel _GameModel;
        private List<CoinTradeNode> TradeList = new();

        private void Awake()
        {
            _SellStoreSystem = this.GetSystem<ISellStoreSystem>();
            _GameModel = this.GetModel<IGameModel>();
        }

        private void Start()
        {
            toggle.onValueChanged.AddListener(Display);
            mainToggle.onValueChanged.AddListener(Display);

            // 生成Trade UI
            for (int index = 0; index < 5; index++)
            {
                var tradeData = _SellStoreSystem.GetCoinTradeByIndex(index);
                // 创建Trade节点
                var trade = TradePrefab.Instantiate().GetComponent<CoinTradeNode>();
                trade.transform.SetParent(Trades, false);
                trade.Show();
                TradeList.Add(trade);

                // 设置TradeUI
                trade.CoinText.text = tradeData.CoinAmount.ToString();
                var lootView = ItemViewFactory.CreateItemView(tradeData.LootData);
                lootView.transform.SetParent(trade.ForSale, false);
                trade.TradeBtn.interactable =
                    _GameModel.GameData.InventoryData.HasTradableCard(tradeData.LootData.PlantId);

                // == 交易事件
                var capturedIndex = index;
                trade.TradeBtn.onClick.AddListener(() =>
                {
                    this.SendCommand<SellTradeCommand>(new SellTradeCommand(capturedIndex));

                    // UI更新
                    var newTrade = _SellStoreSystem.GetCoinTradeByIndex(capturedIndex);
                    trade.CoinText.text = newTrade.CoinAmount.ToString();
                    var definition =
                        PlantConfigReader.GetCardDefinition(newTrade.LootData.PlantId.ToDef());
                    lootView.GetComponent<CardViewController>().Initialize(definition);

                    var tradeNode = TradeList[capturedIndex];
                    var tradeData = _SellStoreSystem.GetCoinTradeByIndex(capturedIndex);
                    tradeNode.TradeBtn.interactable =
                        _GameModel.GameData.InventoryData.HasTradableCard(tradeData.LootData.PlantId);
                });
            }

            // UI变化
            _GameModel.GameData.InventoryData.OnCardAdded.Register((card) =>
                {
                    for (int index = 0; index < 5; index++)
                    {
                        var tradeNode = TradeList[index];
                        var tradeData = _SellStoreSystem.GetCoinTradeByIndex(index);
                        tradeNode.TradeBtn.interactable =
                            _GameModel.GameData.InventoryData.HasTradableCard(tradeData.LootData.PlantId);
                    }
                }
            ).UnRegisterWhenGameObjectDestroyed(this);

            _GameModel.GameData.InventoryData.OnCardRemoved.Register((card) =>
                {
                    for (int index = 0; index < 5; index++)
                    {
                        var tradeNode = TradeList[index];
                        var tradeData = _SellStoreSystem.GetCoinTradeByIndex(index);
                        tradeNode.TradeBtn.interactable =
                            _GameModel.GameData.InventoryData.HasTradableCard(tradeData.LootData.PlantId);
                    }
                }
            ).UnRegisterWhenGameObjectDestroyed(this);
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