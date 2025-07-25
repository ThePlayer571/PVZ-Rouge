using TPL.PVZR.Classes.DataClasses.Loot;

namespace TPL.PVZR.Classes.DataClasses.CoinTrade
{
    public class CoinTradeData
    {
        public int CoinAmount { get; }
        public LootData LootData { get; }

        public bool Used { get; set; }

        public CoinTradeData(int coinAmount, LootData lootData)
        {
            CoinAmount = coinAmount;
            LootData = lootData;
            Used = false;
        }

        public CoinTradeData(CoinTradeInfo coinTradeInfo)
            : this(coinTradeInfo.coinAmount, LootData.Create(coinTradeInfo.lootInfo))
        {
        }

        public CoinTradeData(CoinTradeData coinTradeData)
            : this(coinTradeData.CoinAmount, coinTradeData.LootData)
        {
        }
    }
}