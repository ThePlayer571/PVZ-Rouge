using System;
using System.Collections.Generic;
using System.Linq;
using QAssetBundle;
using QFramework;
using TPL.PVZR.Classes;
using TPL.PVZR.Classes.DataClasses;
using TPL.PVZR.Classes.DataClasses.Loot;
using TPL.PVZR.Classes.DataClasses.Recipe;
using TPL.PVZR.Helpers.ClassCreator.Item;
using TPL.PVZR.Tools;
using TPL.PVZR.Tools.Random;

namespace TPL.PVZR.Helpers.ClassCreator
{
    public static class CoinTradeHelper
    {
        static CoinTradeHelper()
        {
            var resLoader = ResLoader.Allocate();
            var itemValueList =
                resLoader.LoadSync<ItemValueList>(Listconfigs.BundleName, Listconfigs.ItemValueList);
            foreach (var plant in itemValueList.plantValueList)
            {
                _coinTrades.Add(new CoinTradeGenerateInfo
                {
                    weight = 10,
                    coinTradeInfo = new CoinTradeInfo
                    {
                        coinAmount = plant.Value,
                        lootInfo = new LootInfo { LootType = LootType.Card, PlantId = plant.Key }
                    }
                });
            }

            foreach (var plantBook in itemValueList.plantBookValueList)
            {
                _coinTrades.Add(new CoinTradeGenerateInfo
                {
                    weight = 10,
                    onlyOnce = true,
                    coinTradeInfo = new CoinTradeInfo
                    {
                        coinAmount = plantBook.Value,
                        lootInfo = new LootInfo { LootType = LootType.PlantBook, PlantBookId = plantBook.Key }
                    }
                });
            }
        }

        private static readonly List<CoinTradeGenerateInfo> _coinTrades = new();

        public static CoinTradeData CreateCoinTradeData(PlantId plantId, float multiplier = 1f)
        {
            var targetInfo = _coinTrades.FirstOrDefault(trade => trade.coinTradeInfo.lootInfo.PlantId == plantId);
            if (targetInfo == null)
            {
                throw new ArgumentException($"未找到的PlantId: {plantId}，可能是未配置导致");
            }

            return new CoinTradeData(targetInfo.coinTradeInfo, multiplier);
        }

        public static RandomPool<CoinTradeGenerateInfo, CoinTradeInfo> GetRelatedCoinTradePool(
            HashSet<PlantId> plantIds)
        {
            var relatedCoinTrades = new List<CoinTradeGenerateInfo>();
            foreach (var coinTradeGenerateInfo in _coinTrades)
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
            return new RandomPool<CoinTradeGenerateInfo, CoinTradeInfo>(_coinTrades, 1, RandomHelper.Game);
        }
    }
}