using System;
using System.Collections.Generic;
using QFramework;
using TPL.PVZR.CommandEvents.__NewlyAdded__;
using TPL.PVZR.Helpers;
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
        private List<RecipeTradeNode> TradeList = new();

        private void Awake()
        {
            _StoreSystem = this.GetSystem<IStoreSystem>();
        }

        private void Start()
        {
            toggle.onValueChanged.AddListener(Display);
            mainToggle.onValueChanged.AddListener(Display);

            // 创建Trades
            foreach (var recipeData in _StoreSystem.ActiveRecipes)
            {
                // 创建Trade节点
                var trade = TradePrefab.Instantiate().GetComponent<RecipeTradeNode>();
                trade.transform.SetParent(Trades, false);
                trade.Show();
                TradeList.Add(trade);

                // 订阅交易事件
                var capturedRecipeData = recipeData;
                trade.TradeBtn.onClick.AddListener(() =>
                {
                    this.SendCommand<BarterCommand>(new BarterCommand(capturedRecipeData));
                });

                // 填充Ingredients
                foreach (var consumePlant in recipeData.consumeCards)
                {
                    var cardView = ItemViewFactory.CreateItemView(consumePlant);
                    cardView.transform.SetParent(trade.Ingredients, false);
                }

                // 填充Output
                var outputCard = ItemViewFactory.CreateItemView(recipeData.output);
                outputCard.transform.SetParent(trade.Output, false);
            }
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