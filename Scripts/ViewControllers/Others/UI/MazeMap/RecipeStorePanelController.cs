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
    public class RecipeStorePanelController : MonoBehaviour, IController
    {
        [SerializeField] private Toggle mainToggle;
        [SerializeField] private Toggle toggle;
        [SerializeField] private RectTransform View;

        [SerializeField] private RectTransform Trades;
        [SerializeField] private GameObject TradePrefab;

        private IStoreSystem _StoreSystem;
        private IGameModel _GameModel;
        private List<RecipeTradeNode> TradeList = new();

        private void Awake()
        {
            _StoreSystem = this.GetSystem<IStoreSystem>();
            _GameModel = this.GetModel<IGameModel>();
        }

        private void Start()
        {
            toggle.onValueChanged.AddListener(Display);
            mainToggle.onValueChanged.AddListener(Display);

            // 创建Trades
            for (int index = 0; index < 8; index++)
            {
                var recipeData = _StoreSystem.GetRecipeByIndex(index);
                // 创建Trade节点
                var trade = TradePrefab.Instantiate().GetComponent<RecipeTradeNode>();
                trade.transform.SetParent(Trades, false);
                trade.Show();
                TradeList.Add(trade);

                // 填充Ingredients (不要调换顺序，我希望按照这个顺序显示)
                // plant
                foreach (var consumePlant in recipeData.consumeCards)
                {
                    var cardView = ItemViewFactory.CreateItemView(consumePlant);
                    cardView.transform.SetParent(trade.Ingredients, false);
                }
                // coin
                if (recipeData.consumeCoins > 0)
                {
                    var coinView = ItemViewFactory.CreateItemView(recipeData.consumeCoins);
                    coinView.transform.SetParent(trade.Ingredients, false);
                }

                // 填充Output
                var outputCard = ItemViewFactory.CreateItemView(recipeData.output);
                outputCard.transform.SetParent(trade.Output, false);

                // 初始化UI
                trade.TradeBtn.interactable =
                    !recipeData.used && _GameModel.GameData.InventoryData.CanAfford(recipeData);

                // == 事件订阅
                // 交易事件
                var capturedIndex = index;
                trade.TradeBtn.onClick.AddListener(() =>
                {
                    var recipe = _StoreSystem.GetRecipeByIndex(capturedIndex);
                    if (recipe.used || !_GameModel.GameData.InventoryData.CanAfford(recipe)) return;

                    this.SendCommand<BarterCommand>(new BarterCommand(capturedIndex));
                    
                    // UI改变
                    outputCard.Hide();
                });
            }

            // UI更新事件
            _GameModel.GameData.InventoryData.OnCardAdded.Register((card) =>
                {
                    for (int index = 0; index < 8; index++)
                    {
                        var tradeNode = TradeList[index];
                        var recipeData = _StoreSystem.GetRecipeByIndex(index);
                        tradeNode.TradeBtn.interactable =
                            !recipeData.used && _GameModel.GameData.InventoryData.CanAfford(recipeData);
                    }
                }
            ).UnRegisterWhenGameObjectDestroyed(this);
            _GameModel.GameData.InventoryData.OnCardRemoved.Register((card) =>
                {
                    for (int index = 0; index < 8; index++)
                    {
                        var tradeNode = TradeList[index];
                        var recipeData = _StoreSystem.GetRecipeByIndex(index);
                        tradeNode.TradeBtn.interactable =
                            !recipeData.used && _GameModel.GameData.InventoryData.CanAfford(recipeData);
                    }
                }
            ).UnRegisterWhenGameObjectDestroyed(this);
        }

        private void OnDestroy()
        {
            toggle.onValueChanged.RemoveListener(Display);
            mainToggle.onValueChanged.RemoveListener(Display);
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