using System.Collections.Generic;
using QFramework;
using TMPro;
using TPL.PVZR.Classes.DataClasses.CoinTrade;
using TPL.PVZR.Classes.DataClasses.Loot;
using TPL.PVZR.CommandEvents.__NewlyAdded__;
using TPL.PVZR.Helpers.New.GameObjectFactory;
using TPL.PVZR.Models;
using TPL.PVZR.Systems.MazeMap;
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
        [SerializeField] private Button RefreshBtn;
        [SerializeField] private TextMeshProUGUI RefreshCostText;

        private ICoinStoreSystem _CoinStoreSystem;
        private IGameModel _GameModel;
        private List<CoinTradeNode> TradeNodeList = new();

        private void Awake()
        {
            _CoinStoreSystem = this.GetSystem<ICoinStoreSystem>();
            _GameModel = this.GetModel<IGameModel>();
        }


        private bool ShouldBeAvailable(CoinTradeData tradeData, int? coin = null, int? seedSlotCount = null,
            int? cardCount = null)
        {
            if (tradeData.Used) return false;

            var inventory = _GameModel.GameData.InventoryData;
            coin ??= inventory.Coins.Value;
            if (coin < tradeData.CoinAmount) return false;

            switch (tradeData.LootData.LootType)
            {
                case LootType.SeedSlot:
                    seedSlotCount ??= inventory.SeedSlotCount.Value;
                    return seedSlotCount < inventory.MaxSeedSlotCount;
                case LootType.Card:
                    cardCount ??= inventory.Cards.Count;
                    return cardCount < inventory.MaxCardCount;
                default:
                    return true;
            }
        }

        private void Start()
        {
            toggle.onValueChanged.AddListener(Display);
            mainToggle.onValueChanged.AddListener(Display);

            RefreshBtn.onClick.AddListener(() => { this.SendCommand<CoinStoreRefreshCommand>(); });

            for (int index = 0; index < 10; index++)
            {
                // 这个tradeData承担引用的责任（实则是良性史山）
                var capturedIndex = index;
                var tradeData = _CoinStoreSystem.GetCoinTradeByIndex(capturedIndex);
                // 创建Trade节点
                var trade = TradePrefab.Instantiate().GetComponent<CoinTradeNode>();
                trade.transform.SetParent(Trades, false);
                trade.Show();
                TradeNodeList.Add(trade);

                // 初始化UI
                var lootView = ItemViewFactory.CreateItemView(tradeData.LootData);
                lootView.transform.SetParent(trade.ForSale, false);
                trade.CoinText.text = tradeData.CoinAmount.ToString();
                trade.TradeBtn.interactable = ShouldBeAvailable(tradeData);

                if (tradeData.Used)
                {
                    lootView.Hide();
                }

                // UI变化事件
                _CoinStoreSystem.OnRewrite.Register(() =>
                {
                    // 重新获取TradeData
                    tradeData = _CoinStoreSystem.GetCoinTradeByIndex(capturedIndex);
                    // 销毁旧的UI
                    lootView.gameObject.DestroySelf();
                    // 重新创建UI
                    lootView = ItemViewFactory.CreateItemView(tradeData.LootData);
                    lootView.transform.SetParent(trade.ForSale, false);
                    trade.CoinText.text = tradeData.CoinAmount.ToString();
                    trade.TradeBtn.interactable = ShouldBeAvailable(tradeData);
                }).UnRegisterWhenGameObjectDestroyed(this);

                _GameModel.GameData.InventoryData.Coins.Register(val =>
                {
                    trade.TradeBtn.interactable = ShouldBeAvailable(tradeData, coin: val);
                }).UnRegisterWhenGameObjectDestroyed(this);

                if (tradeData.LootData.LootType == LootType.SeedSlot)
                {
                    _GameModel.GameData.InventoryData.SeedSlotCount.RegisterWithInitValue(val =>
                        {
                            trade.TradeBtn.interactable = ShouldBeAvailable(tradeData, seedSlotCount: val);
                        })
                        .UnRegisterWhenGameObjectDestroyed(this);
                }
                else if (tradeData.LootData.LootType == LootType.Card)
                {
                    _GameModel.GameData.InventoryData.OnCardCountChange.Register(val =>
                        {
                            trade.TradeBtn.interactable = ShouldBeAvailable(tradeData, cardCount: val);
                        })
                        .UnRegisterWhenGameObjectDestroyed(this);
                }


                // == 交易事件
                trade.TradeBtn.onClick.AddListener(() =>
                    {
                        this.SendCommand<CoinTradeCommand>(new CoinTradeCommand(capturedIndex));

                        // UI改变
                        lootView.Hide();
                    }
                );
            }

            _GameModel.GameData.InventoryData.Coins.RegisterWithInitValue(coin =>
            {
                RefreshBtn.interactable = coin >= _CoinStoreSystem.CurrentRefreshCost;
            }).UnRegisterWhenGameObjectDestroyed(this);

            // todo 不精确，应该是订阅刷新的事件，现在的代码只能说是暂时不出错
            _CoinStoreSystem.OnRewrite.Register(() =>
            {
                RefreshBtn.interactable =
                    _GameModel.GameData.InventoryData.Coins.Value >= _CoinStoreSystem.CurrentRefreshCost;
                // 这个也是，应该订阅RefreshCost才对
                RefreshCostText.text = "Cost: " + _CoinStoreSystem.CurrentRefreshCost.ToString();
            }).UnRegisterWhenGameObjectDestroyed(this);
        }

        private void OnDestroy()
        {
            toggle.onValueChanged.RemoveListener(Display);
            mainToggle.onValueChanged.RemoveListener(Display);

            RefreshBtn.onClick.RemoveAllListeners();

            // 清理TradeList
            foreach (var trade in TradeNodeList)
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