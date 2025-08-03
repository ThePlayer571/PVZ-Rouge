using System;
using QFramework;
using TPL.PVZR.Models;
using TPL.PVZR.Systems.MazeMap;
using TPL.PVZR.Tools.SoyoFramework;

namespace TPL.PVZR.CommandEvents.__NewlyAdded__
{
    public struct CoinStoreRefreshEvent : IServiceEvent
    {
    }

    public class CoinStoreRefreshCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            var _GameModel = this.GetModel<IGameModel>();
            var _CoinStoreSystem = this.GetSystem<ICoinStoreSystem>();

            // 异常处理
            if (_GameModel.GameData.InventoryData.Coins.Value < _CoinStoreSystem.CurrentRefreshCost)
                throw new Exception(
                    $"尝试刷新商店，但金币不足: {_GameModel.GameData.InventoryData.Coins.Value} < {_CoinStoreSystem.CurrentRefreshCost}");

            //
            this.SendEvent<CoinStoreRefreshEvent>();
        }
    }
}