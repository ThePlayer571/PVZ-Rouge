using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using QFramework;
using TPL.PVZR.Classes.DataClasses.CoinTrade;
using TPL.PVZR.Classes.DataClasses.Loot;
using TPL.PVZR.CommandEvents.__NewlyAdded__;
using TPL.PVZR.CommandEvents.Phase;
using TPL.PVZR.Helpers.New;
using TPL.PVZR.Helpers.New.ClassCreator;
using TPL.PVZR.Models;
using TPL.PVZR.Tools.SoyoFramework;
using UnityEngine;
using UnityEngineInternal;

namespace TPL.PVZR.Systems.MazeMap
{
    public interface ICoinStoreSystem : IServiceManageSystem, IDataSystem
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
        private int _refreshCount = 0;


        private void AutoWriteCoinTrades()
        {
            _activeTrades.Clear();

            for (int i = 0; i < 10; i++)
            {
                var coinTradeData = TradeCreator.CreateRandomCoinTradeDataByMazeMap(_refreshCount * 5);
                // 卡槽上限
                if (!IsAllowingTrade(coinTradeData))
                {
                    i--;
                    continue;
                }

                _activeTrades.Add(coinTradeData);
            }

            if (_hasTradeData) OnRewrite.Trigger();
            _hasTradeData = true;
        }

        private bool IsAllowingTrade(CoinTradeData tradeData)
        {
            // 卡槽上限
            if (tradeData.LootData.LootType == LootType.SeedSlot &&
                !_GameModel.GameData.InventoryData.HasAvailableSeedSlotSlots())
                return false;
            // 秘籍重复
            if (tradeData.LootData.LootType == LootType.PlantBook &&
                _GameModel.GameData.InventoryData.HasPlantBook(tradeData.LootData.PlantBookId))
                return false;

            return true;
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
                    case GamePhase.GameInitialization:
                        switch (e.PhaseStage)
                        {
                            case PhaseStage.EnterNormal:
                                TradeCreator.InitializeCoinTradeGenerator(_GameModel.GameData.MazeMapData);
                                break;
                        }

                        break;
                    case GamePhase.MazeMapInitialization:
                        switch (e.PhaseStage)
                        {
                            case PhaseStage.EnterNormal:
                                _refreshCount = 0;
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
                _refreshCount++;

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

        public int RefreshCost => 10 * (1 << _refreshCount);
    }
}