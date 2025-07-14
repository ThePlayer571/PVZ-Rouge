using TPL.PVZR.Classes.DataClasses.Loot;
using TPL.PVZR.Helpers.ClassCreator.Item;

namespace TPL.PVZR.Classes.DataClasses
{
    public class CoinTradeData
    {
        public int CoinAmount { get; }
        public LootData LootData { get; }
        
        public bool used { get; set; }

        public CoinTradeData(CoinTradeInfo coinTradeInfo)
        {
            CoinAmount = coinTradeInfo.coinAmount;
            LootData = LootHelper.CreateLootData(coinTradeInfo.lootInfo);
            used = false;
        }
    }
}