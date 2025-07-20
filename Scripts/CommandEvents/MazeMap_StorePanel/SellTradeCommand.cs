using System;
using System.Linq;
using QFramework;
using TPL.PVZR.Models;
using TPL.PVZR.Systems;
using TPL.PVZR.Systems.MazeMap;

namespace TPL.PVZR.CommandEvents.__NewlyAdded__
{
    public struct SellTradeEvent
    {
        public int index;
    }

    public class SellTradeCommand : AbstractCommand
    {
        public SellTradeCommand(int index)
        {
            _index = index;
        }

        private int _index;

        protected override void OnExecute()
        {
            var _GameModel = this.GetModel<IGameModel>();
            var _SellStoreSystem = this.GetSystem<ISellStoreSystem>();

            // 异常处理
            var trade = _SellStoreSystem.GetCoinTradeByIndex(_index);
            var inventory = _GameModel.GameData.InventoryData;
            if (!inventory.Cards.Any(cardData => !cardData.Locked && cardData.CardDefinition.PlantDef.Id == trade.LootData.PlantId))
                throw new Exception($"没有对应的卡牌，无法进行交易，index: {_index}");

            // 
            this.SendEvent<SellTradeEvent>(new SellTradeEvent { index = _index });
        }
    }
}