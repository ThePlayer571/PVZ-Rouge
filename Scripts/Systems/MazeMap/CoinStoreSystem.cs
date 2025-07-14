using System.Collections.Generic;
using System.Linq;
using QFramework;
using TPL.PVZR.Classes.DataClasses;
using TPL.PVZR.Classes.DataClasses.Loot;
using TPL.PVZR.CommandEvents.__NewlyAdded__;
using TPL.PVZR.CommandEvents.Phase;
using TPL.PVZR.Helpers.ClassCreator;
using TPL.PVZR.Helpers.ClassCreator.Item;
using TPL.PVZR.Models;

namespace TPL.PVZR.Systems
{
    public interface ICoinStoreSystem : ISystem
    {
        CoinTradeData GetCoinTradeByIndex(int index);
    }

    public class CoinStoreSystem : AbstractSystem, ICoinStoreSystem
    {
        private IGameModel _GameModel;

        private void AutoWriteCoinTrades()
        {
            _activeTrades.Clear();

            var pool = CoinTradeHelper.GetAllCoinTradePool();
            var _ = pool.GetRandomOutputs(15).Select(info => new CoinTradeData(info));
            _activeTrades.AddRange(_);
        }

        private void ClearCoinTrades()
        {
            _activeTrades.Clear();
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

            this.RegisterEvent<CoinTradeEvent>(e =>
            {
                var tradeData = GetCoinTradeByIndex(e.index);
                tradeData.used = true;
                var inventory = _GameModel.GameData.InventoryData;

                // 消耗
                inventory.Coins.Value -= tradeData.CoinAmount;

                // 添加
                switch (tradeData.LootData.LootType)
                {
                    case LootType.Card:
                        var cardData =
                            CardHelper.CreateCardData(PlantBookHelper.GetPlantDef(tradeData.LootData.PlantId));
                        inventory.AddCard(cardData);
                        break;
                    case LootType.PlantBook:
                        var plantBookData =
                            PlantBookHelper.CreatePlantBookData(tradeData.LootData.PlantBookId);
                        inventory.AddPlantBook(plantBookData);
                        break;
                }
            });
        }

        public CoinTradeData GetCoinTradeByIndex(int index)
        {
            return _activeTrades[index];
        }
    }
}