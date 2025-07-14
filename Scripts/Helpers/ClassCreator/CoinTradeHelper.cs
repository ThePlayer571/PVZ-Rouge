using System.Collections.Generic;
using QAssetBundle;
using QFramework;
using TPL.PVZR.Classes;
using TPL.PVZR.Classes.DataClasses;
using TPL.PVZR.Classes.DataClasses.Loot;
using TPL.PVZR.Classes.DataClasses.Recipe;
using TPL.PVZR.Tools;
using TPL.PVZR.Tools.Random;

namespace TPL.PVZR.Helpers.ClassCreator
{
    public static class CoinTradeHelper
    {
        static CoinTradeHelper()
        {
            var resLoader = ResLoader.Allocate();
            Default = resLoader.LoadSync<CoinTradeListConfig>(Listconfigs.BundleName, Listconfigs.CoinTradeListConfig);
        }

        private static CoinTradeListConfig Default;

        public static RandomPool<CoinTradeGenerateInfo, CoinTradeInfo> GetRelatedCoinTradePool(HashSet<PlantId> plantIds)
        {
            var relatedCoinTrades = new List<CoinTradeGenerateInfo>();
            foreach (var coinTradeGenerateInfo in Default.coinTrades)
            {
                if (coinTradeGenerateInfo.coinTradeInfo.lootInfo.LootType != LootType.Card) continue;
                
                if (plantIds.Contains(coinTradeGenerateInfo.coinTradeInfo.lootInfo.PlantId))
                {
                    relatedCoinTrades.Add(coinTradeGenerateInfo);
                }
            }

            return new RandomPool<CoinTradeGenerateInfo, CoinTradeInfo>(relatedCoinTrades, 1, RandomHelper.Game);
        }

        public static RandomPool<CoinTradeGenerateInfo, CoinTradeInfo> GetAllCoinTradePool()
        {
            // RandomPool会自动ToList()，不用担心SO被修改
            return new RandomPool<CoinTradeGenerateInfo, CoinTradeInfo>(Default.coinTrades, 1, RandomHelper.Game);
        }
    }
}