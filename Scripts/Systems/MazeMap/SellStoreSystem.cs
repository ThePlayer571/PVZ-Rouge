using System.Collections.Generic;
using System.Linq;
using QFramework;
using TPL.PVZR.Classes;
using TPL.PVZR.Classes.DataClasses;
using TPL.PVZR.Classes.DataClasses.Loot;
using TPL.PVZR.CommandEvents.__NewlyAdded__;
using TPL.PVZR.CommandEvents.Phase;
using TPL.PVZR.Helpers.ClassCreator;
using TPL.PVZR.Helpers.ClassCreator.Item;
using TPL.PVZR.Models;
using TPL.PVZR.Tools.Random;

namespace TPL.PVZR.Systems
{
    public interface ISellStoreSystem : ISystem
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

        private void AutoWriteCoinTrades()
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
                return CoinTradeHelper.CreateCoinTradeData(chosenPlant, 0.4f);
            }
            else
            {
                var chosenPlant = RandomHelper.Game.RandomChoose(
                    _GameModel.GameData.InventoryData.Cards
                        .Where(data => !data.Locked)
                        .Select(cardData => cardData.CardDefinition.PlantDef.Id));
                return CoinTradeHelper.CreateCoinTradeData(chosenPlant, 0.4f);
            }
        }

        private void ClearCoinTrades()
        {
            _activeTrades.Clear();
            Available = false;
        }


        private List<CoinTradeData> _activeTrades { get; } = new List<CoinTradeData>();

        protected override void OnInit()
        {
            _GameModel = this.GetModel<IGameModel>();

            this.RegisterEvent<OnPhaseChangeEvent>(e =>
            {
                switch (e.GamePhase)
                {
                    case GamePhase.MazeMapInitialization:
                        switch (e.PhaseStage)
                        {
                            case PhaseStage.EnterNormal:
                                AutoWriteCoinTrades();
                                break;
                        }

                        break;
                    case GamePhase.MazeMap:
                        switch (e.PhaseStage)
                        {
                            case PhaseStage.LeaveLate:
                                ClearCoinTrades();
                                break;
                        }

                        break;
                }
            });

            this.RegisterEvent<SellTradeEvent>(e =>
            {
                var trade = GetCoinTradeByIndex(e.index);
                var inventory = _GameModel.GameData.InventoryData;

                // 消耗
                inventory.RemoveCard(inventory.Cards.First(cardData =>
                    !cardData.Locked && cardData.CardDefinition.PlantDef.Id == trade.LootData.PlantId));

                // 添加
                inventory.Coins.Value += trade.CoinAmount;

                // 重新生成售卖对象
                AutoWriteIndex(e.index);
            });
        }

        public CoinTradeData GetCoinTradeByIndex(int index)
        {
            return _activeTrades[index];
        }

        public bool Available { get; private set; }
    }
}