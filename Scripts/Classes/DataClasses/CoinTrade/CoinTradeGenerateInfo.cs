using System;
using TPL.PVZR.Tools;

namespace TPL.PVZR.Classes.DataClasses.CoinTrade
{
    [Serializable]
    public class CoinTradeGenerateInfo : IGenerateInfo<CoinTradeInfo>
    {
        public float weight;
        public CoinTradeInfo coinTradeInfo;
        public bool onlyOnce = false;

        public float Value => 0;
        public float Weight => weight;
        public CoinTradeInfo Output => coinTradeInfo;
        public bool OnlyOnce => onlyOnce;
    }
}