using System;
using System.Collections.Generic;
using System.Linq;
using QFramework;
using TPL.PVZR.Classes.DataClasses.CoinTrade;
using TPL.PVZR.CommandEvents.__NewlyAdded__;
using TPL.PVZR.Helpers.New.ClassCreator;
using TPL.PVZR.Models;
using TPL.PVZR.Services;
using TPL.PVZR.Tools.Random;
using TPL.PVZR.Tools.SoyoFramework;

namespace TPL.PVZR.Systems.MazeMap
{
    public interface ISellStoreSystem : IDataSystem
    {
        CoinTradeData GetCoinTradeByIndex(int index);
    }


    public class SellStoreSystem : AbstractSystem, ISellStoreSystem
    {
        private IGameModel _GameModel;

        private void AutoWriteIndex(int index)
        {
            _activeTrades[index] = GetRandomCoinTradeData();
        }

        private void AutoWriteSellTrades()
        {
            _activeTrades.Clear();

            for (int i = 0; i < 5; i++)
            {
                var coinTradeData = GetRandomCoinTradeData();
                _activeTrades.Add(coinTradeData);
            }
        }

        private CoinTradeData GetRandomCoinTradeData()
        {
            if (_GameModel.GameData.InventoryData.Cards.All(cardData => cardData.Locked))
            {
                var chosenPlant = RandomHelper.Game.RandomChoose(
                    _GameModel.GameData.InventoryData.Cards
                        .Select(cardData => cardData.CardDefinition.PlantDef.Id));
                return TradeCreator.CreateCoinTradeDataWithRandomVariation(chosenPlant, 0.4f);
            }
            else
            {
                var chosenPlant = RandomHelper.Game.RandomChoose(
                    _GameModel.GameData.InventoryData.Cards
                        .Where(data => !data.Locked)
                        .Select(cardData => cardData.CardDefinition.PlantDef.Id));
                return TradeCreator.CreateCoinTradeDataWithRandomVariation(chosenPlant, 0.4f);
            }
        }

        private void ClearSellTrades()
        {
            _activeTrades.Clear();
        }


        private List<CoinTradeData> _activeTrades { get; } = new List<CoinTradeData>();

        protected override void OnInit()
        {
            _GameModel = this.GetModel<IGameModel>();

            var phaseService = this.GetService<IPhaseService>();

            phaseService.RegisterCallBack((GamePhase.MazeMapInitialization, PhaseStage.EnterEarly), e =>
            {
                var notRefresh = (bool)e.Paras.GetValueOrDefault<string, object>("NotRefresh", false);
                if (notRefresh) return;
                //
                AutoWriteSellTrades();
            });
            phaseService.RegisterCallBack((GamePhase.GameExiting, PhaseStage.LeaveNormal), e =>
            {
                ClearSellTrades();
            });


            this.RegisterEvent<SellTradeEvent>(e =>
            {
                var inventory = _GameModel.GameData.InventoryData;
                var trade = GetCoinTradeByIndex(e.index);
                // 重新生成售卖对象
                AutoWriteIndex(e.index);
                // 消耗
                inventory.RemoveCard(inventory.Cards.First(cardData =>
                    !cardData.Locked && cardData.CardDefinition.PlantDef.Id == trade.LootData.PlantId));
                // 添加
                inventory.Coins.Value += trade.CoinAmount;
            });
        }

        public CoinTradeData GetCoinTradeByIndex(int index)
        {
            try
            {
                return _activeTrades[index];
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ArgumentOutOfRangeException(nameof(index),
                    $"试图访问超出范围的索引: {index}，而当前列表长度为: {_activeTrades.Count}");
            }
        }
    }
}