using System;
using TPL.PVZR.Classes.DataClasses.Loot;

namespace TPL.PVZR.Classes.DataClasses.CoinTrade
{
    [Serializable]
    public class CoinTradeInfo
    {
        public int coinAmount;
        public LootInfo lootInfo;
    }
}