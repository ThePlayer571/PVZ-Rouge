using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using QFramework;
using TPL.PVZR.Classes.DataClasses.CoinTrade;
using TPL.PVZR.Classes.DataClasses.Loot;
using TPL.PVZR.CommandEvents.__NewlyAdded__;
using TPL.PVZR.Helpers.New;
using TPL.PVZR.Helpers.New.ClassCreator;
using TPL.PVZR.Models;
using TPL.PVZR.Services;
using TPL.PVZR.Tools.SoyoFramework;
using UnityEngine;
using UnityEngineInternal;

namespace TPL.PVZR.Systems.MazeMap
{
    public interface ICoinStoreSystem : IDataSystem
    {
        EasyEvent OnRewrite { get; }
        CoinTradeData GetCoinTradeByIndex(int index);

        int CurrentRefreshCost { get; }
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

            var phaseService = this.GetService<IPhaseService>();
            phaseService.RegisterCallBack((GamePhase.GameInitialization, PhaseStage.EnterNormal), e =>
            {
                //
                TradeCreator.InitializeCoinTradeGenerator(_GameModel.GameData.MazeMapData);
            });
            phaseService.RegisterCallBack((GamePhase.MazeMapInitialization, PhaseStage.EnterEarly), e =>
            {
                var notRefresh = (bool)e.Paras.GetValueOrDefault<string, object>("NotRefresh", false);
                if (notRefresh) return;
                _refreshCount = 0;
                AutoWriteCoinTrades();
            });

            this.RegisterEvent<OnCoinStoreRefreshed>(e =>
            {
                _refreshCount++;

                AutoWriteCoinTrades();
            });

            this.RegisterEvent<CoinTradeEvent>(e =>
            {
                var inventory = _GameModel.GameData.InventoryData;
                var tradeData = GetCoinTradeByIndex(e.index);
                tradeData.Used = true;

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

        public int CurrentRefreshCost => RefreshCostOf(_refreshCount + 1);

        public void Refresh()
        {
            throw new NotImplementedException();
        }

        public void DealCoinTrade(int index)
        {
            throw new NotImplementedException();
        }

        public int RefreshCostOf(int refreshCount)
        {
            return refreshCount switch
            {
                1 => 5,
                <= 4 => 10 * (refreshCount - 1),
                _ => 20 + 30 * (refreshCount - 4)
            };
        }
    }
}