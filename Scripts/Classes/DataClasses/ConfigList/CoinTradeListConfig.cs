using System.Collections.Generic;
using UnityEngine;

namespace TPL.PVZR.Classes.DataClasses.Recipe
{
    [CreateAssetMenu(fileName = "CoinTradeListConfig", menuName = "PVZR_Config/CoinTradeListConfig", order = 5)]
    public class CoinTradeListConfig:ScriptableObject
    {
        public List<CoinTradeGenerateInfo> coinTrades;
    }
}