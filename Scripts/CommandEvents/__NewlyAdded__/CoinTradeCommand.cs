using System;
using QFramework;
using TPL.PVZR.Models;
using TPL.PVZR.Systems;
using TPL.PVZR.Systems.MazeMap;

namespace TPL.PVZR.CommandEvents.__NewlyAdded__
{
    public struct CoinTradeEvent
    {
        public int index;
    }


    public class CoinTradeCommand : AbstractCommand

    {
        public CoinTradeCommand(int index)
        {
            _index = index;
        }

        private int _index;

        protected override void OnExecute()
        {
            var _GameModel = this.GetModel<IGameModel>();
            var _CoinStoreSystem = this.GetSystem<ICoinStoreSystem>();

            // 异常处理
            var trade = _CoinStoreSystem.GetCoinTradeByIndex(_index);
            var inventory = _GameModel.GameData.InventoryData;
            if (trade.Used) throw new Exception($"已经进行过交易，index: {_index}");
            if (inventory.Coins.Value < trade.CoinAmount)
                throw new Exception($"金币不足，无法进行交易，index: {_index}");

            // 
            this.SendEvent<CoinTradeEvent>(new CoinTradeEvent { index = _index });
        }
    }
}