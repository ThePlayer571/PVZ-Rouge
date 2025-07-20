using System.Collections.Generic;
using System.Linq;
using QFramework;
using TPL.PVZR.Classes.DataClasses.CoinTrade;
using TPL.PVZR.Classes.DataClasses.Loot;
using TPL.PVZR.CommandEvents.__NewlyAdded__;
using TPL.PVZR.CommandEvents.Phase;
using TPL.PVZR.Helpers.New;
using TPL.PVZR.Helpers.New.ClassCreator;
using TPL.PVZR.Models;

namespace TPL.PVZR.Systems.MazeMap
{
    public interface ICoinStoreSystem : ISystem
    {
        EasyEvent OnRewrite { get; }
        CoinTradeData GetCoinTradeByIndex(int index);
        int RefreshCost { get; }
    }

    public class CoinStoreSystem : AbstractSystem, ICoinStoreSystem
    {
        private IGameModel _GameModel;
        private List<CoinTradeData> _activeTrades { get; } = new List<CoinTradeData>();
        private bool _hasTradeData = false;


        private void AutoWriteCoinTrades()
        {
            _activeTrades.Clear();

            var pool = TradeCreator.CreateAllCoinTradePool();
            var _ = pool.GetRandomOutputs(10).Select(info => new CoinTradeData(info));
            _activeTrades.AddRange(_);

            if (_hasTradeData) OnRewrite.Trigger();
            _hasTradeData = true;
        }

        private void ClearCoinTrades()
        {
            _activeTrades.Clear();
            _hasTradeData = false;
        }


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
                                RefreshCost = 5;
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

            this.RegisterEvent<CoinStoreRefreshEvent>(e =>
            {
                _GameModel.GameData.InventoryData.Coins.Value -= RefreshCost;
                RefreshCost *= 2;

                AutoWriteCoinTrades();
            });

            this.RegisterEvent<CoinTradeEvent>(e =>
            {
                var tradeData = GetCoinTradeByIndex(e.index);
                tradeData.Used = true;
                var inventory = _GameModel.GameData.InventoryData;

                // 消耗
                inventory.Coins.Value -= tradeData.CoinAmount;

                // 添加
                inventory.AddLootAuto(tradeData.LootData);
            });
        }

        public EasyEvent OnRewrite { get; } = new EasyEvent();

        public CoinTradeData GetCoinTradeByIndex(int index)
        {
            return _activeTrades[index];
        }

        public int RefreshCost { get; private set; }
    }
}