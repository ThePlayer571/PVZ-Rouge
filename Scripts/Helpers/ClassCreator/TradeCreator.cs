using System.Collections.Generic;
using System.Linq;
using TPL.PVZR.Classes.DataClasses.CoinTrade;
using TPL.PVZR.Classes.DataClasses.Item.Card;
using TPL.PVZR.Classes.DataClasses.Item.PlantBook;
using TPL.PVZR.Classes.DataClasses.Loot;
using TPL.PVZR.Classes.DataClasses.Recipe;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.Helpers.New.DataReader;
using TPL.PVZR.Tools;
using TPL.PVZR.Tools.Random;

namespace TPL.PVZR.Helpers.New.ClassCreator
{
    public static class TradeCreator
    {
        #region CoinTrade

        private static CoinTradeGenerateInfo CreateCoinTradeGenerateInfo(PlantId plantId)
        {
            var value = TradeConfigReader.GetValueOf(plantId);
            var weight = TradeConfigReader.GetWeightOf(plantId);
            return new CoinTradeGenerateInfo
            {
                weight = weight,
                coinTradeInfo = new CoinTradeInfo
                {
                    coinAmount = value,
                    lootInfo = LootCreator.CreateLootInfo(plantId)
                }
            };
        }
        
        private static CoinTradeGenerateInfo CreateCoinTradeGenerateInfo(PlantBookId bookId)
        {
            var value = TradeConfigReader.GetValueOf(bookId);
            var weight = TradeConfigReader.GetWeightOf(bookId);
            return new CoinTradeGenerateInfo
            {
                weight = weight,
                coinTradeInfo = new CoinTradeInfo
                {
                    coinAmount = value,
                    lootInfo = LootCreator.CreateLootInfo(bookId)
                }
            };
        }

        private static CoinTradeGenerateInfo CreateCoinTradeGenerateInfoOfSeedSlot()
        {
            var (value ,weight) = TradeConfigReader.GetSeedSlotInfo();
            return new CoinTradeGenerateInfo
            {
                weight = weight,
                coinTradeInfo = new CoinTradeInfo
                {
                    coinAmount = value,
                    lootInfo = LootCreator.CreateLootInfo_SeedSlot()
                }
            };
        }

        public static RandomPool<CoinTradeGenerateInfo, CoinTradeInfo> CreateRelatedCoinTradePool(
            IReadOnlyList<PlantId> plantIds)
        {
            var relatedCoinTrades = plantIds.Select(CreateCoinTradeGenerateInfo).ToList();
            
            return new RandomPool<CoinTradeGenerateInfo, CoinTradeInfo>(relatedCoinTrades, 1, RandomHelper.Game);
        }

        public static RandomPool<CoinTradeGenerateInfo, CoinTradeInfo> CreateAllCoinTradePool()
        {
            var coinTrades = new List<CoinTradeGenerateInfo>();
            coinTrades.AddRange(TradeConfigReader.GetAllPlantStandardValues().Keys.Select(CreateCoinTradeGenerateInfo));
            coinTrades.AddRange(TradeConfigReader.GetAllPlantBookStandardValues().Keys.Select(CreateCoinTradeGenerateInfo));
            coinTrades.Add(CreateCoinTradeGenerateInfoOfSeedSlot());

            return new RandomPool<CoinTradeGenerateInfo, CoinTradeInfo>(coinTrades, 1, RandomHelper.Game);
        }

        public static CoinTradeData CreateCoinTradeData(PlantId plantId, float multiplier = 1f,
            float randomVariationRange = 0.1f)
        {
            var coinTradeGenerateInfo = CreateCoinTradeGenerateInfo(plantId);
            return CreateCoinTradeDataWithRandomVariation(coinTradeGenerateInfo.coinTradeInfo, multiplier, randomVariationRange);
        }

        public static CoinTradeData CreateCoinTradeDataWithRandomVariation(CoinTradeInfo coinTradeInfo, float multiplier = 1f, float randomVariationRange = 0.1f)
        {
            var calculatedCoinAmount = (int)(coinTradeInfo.coinAmount * multiplier *
                      (1 + RandomHelper.Game.Range(-randomVariationRange, randomVariationRange)));
            var lootData = LootData.Create(coinTradeInfo.lootInfo);
            return new CoinTradeData(calculatedCoinAmount, lootData);
        }

        public static CoinTradeData CreateCoinTradeData(CoinTradeInfo coinTradeInfo)
        {
            var lootData = LootData.Create(coinTradeInfo.lootInfo);
            return new CoinTradeData(coinTradeInfo.coinAmount, lootData);
        }

        #endregion

        #region Recipe

        public static RandomPool<RecipeGenerateInfo, RecipeInfo> CreateRelatedRecipePool(HashSet<PlantId> plantIds)
        {
            var relatedRecipes = new List<RecipeGenerateInfo>();
            foreach (var recipeGenerateInfo in TradeConfigReader.GetAllRecipeGenerateInfos())
            {
                foreach (var recipePlantId in recipeGenerateInfo.recipeInfo.inputCards)
                {
                    if (plantIds.Contains(recipePlantId))
                    {
                        relatedRecipes.Add(recipeGenerateInfo);
                        break;
                    }
                }
            }

            return new RandomPool<RecipeGenerateInfo, RecipeInfo>(relatedRecipes, 1, RandomHelper.Game);
        }

        #endregion
    }
}