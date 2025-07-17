using TPL.PVZR.Classes.DataClasses.Loot;
using TPL.PVZR.Tools.Random;

namespace TPL.PVZR.Classes.DataClasses.CoinTrade
{
    public class CoinTradeData
    {
        public int CoinAmount { get; }
        public LootData LootData { get; }

        public bool Used { get; set; }

        public CoinTradeData(CoinTradeInfo coinTradeInfo, float multiplier = 1f, float randomVariationRange = 0.1f)
        {
            CoinAmount =
                (int)(coinTradeInfo.coinAmount * multiplier *
                      (1 + RandomHelper.Game.Range(-randomVariationRange, randomVariationRange)));
            LootData = LootData.Create(coinTradeInfo.lootInfo);
            Used = false;
        }
    }
}