using System.Collections.Generic;
using System.Linq;
using QFramework;
using TPL.PVZR.Classes.DataClasses.CoinTrade;
using TPL.PVZR.Classes.DataClasses.Item.Card;
using TPL.PVZR.Classes.DataClasses.Item.PlantBook;
using TPL.PVZR.Classes.DataClasses.Loot;
using TPL.PVZR.Classes.DataClasses.Recipe;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.Classes.LootPool;
using TPL.PVZR.Classes.MazeMap;
using TPL.PVZR.Helpers.New.DataReader;
using TPL.PVZR.Tools;
using TPL.PVZR.Tools.Random;

namespace TPL.PVZR.Helpers.New.ClassCreator
{
    public static class TradeCreator
    {
        #region CoinTrade

        #region CoinTradeGenerateInfo

        private static CoinTradeGenerateInfo CreateCoinTradeGenerateInfo(PlantId plantId)
        {
            var value = EconomyConfigReader.GetValueOf(plantId);
            var weight = EconomyConfigReader.GetWeightOf(plantId);
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
            var value = EconomyConfigReader.GetValueOf(bookId);
            var weight = EconomyConfigReader.GetWeightOf(bookId);
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
            var value = EconomyConfigReader.GetValueOfSeedSlot();
            var weight = EconomyConfigReader.GetWeightOfSeedSlot();
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

        #endregion

        #region CoinTradeData

        public static CoinTradeData CreateCoinTradeDataWithRandomVariation(PlantId plantId, float valueMultiplier = 1f,
            float randomVariationRange = 0.1f)
        {
            var coinTradeGenerateInfo = CreateCoinTradeGenerateInfo(plantId);
            return CreateCoinTradeDataWithRandomVariation(coinTradeGenerateInfo.coinTradeInfo, valueMultiplier,
                randomVariationRange);
        }

        public static CoinTradeData CreateCoinTradeDataWithRandomVariation(CoinTradeInfo coinTradeInfo,
            float valueMultiplier = 1f, float randomVariationRange = 0.1f)
        {
            var calculatedCoinAmount = (int)(coinTradeInfo.coinAmount * valueMultiplier *
                                             (1 + RandomHelper.Game.Range(-randomVariationRange,
                                                 randomVariationRange)));
            var lootData = LootData.Create(coinTradeInfo.lootInfo);
            return new CoinTradeData(calculatedCoinAmount, lootData);
        }

        public static CoinTradeData CreateCoinTradeData(CoinTradeInfo coinTradeInfo)
        {
            var lootData = LootData.Create(coinTradeInfo.lootInfo);
            return new CoinTradeData(coinTradeInfo.coinAmount, lootData);
        }

        #endregion

        #endregion

        #region CoinTradeRandomPool

        private static RandomPool<LootPoolInfo, LootPoolInfo> _lootPoolPool = null;

        private static Dictionary<LootPoolId, RandomPool<CoinTradeGenerateInfo, CoinTradeInfo>> _coinTradePoolDict =
            new();

        public static void InitializeCoinTradeGenerator(IMazeMapData mazeMapData)
        {
            _coinTradePoolDict.Clear();
            var _ = new List<LootPoolInfo>();
            foreach (var poolDef in mazeMapData.LootPools)
            {
                var id = poolDef.Id;
                var lootPool = LootPoolConfigReader.GetLootPoolInfo(id);
                _.Add(lootPool);
                var coinTrades = new List<CoinTradeGenerateInfo>();
                coinTrades.AddRange(lootPool.cards.Select(CreateCoinTradeGenerateInfo));
                coinTrades.AddRange(lootPool.plantBooks.Select(CreateCoinTradeGenerateInfo));
                if (lootPool.seedSlot) coinTrades.Add(CreateCoinTradeGenerateInfoOfSeedSlot());
                _coinTradePoolDict.Add(id,
                    new RandomPool<CoinTradeGenerateInfo, CoinTradeInfo>(coinTrades, 1, RandomHelper.Game));
            }

            _lootPoolPool = new RandomPool<LootPoolInfo, LootPoolInfo>(_, 1, RandomHelper.Game);
        }

        public static CoinTradeData CreateRandomCoinTradeDataByMazeMap()
        {
            var lootPoolId = _lootPoolPool.GetRandomOutput().lootPoolDef.Id;
            return CreateCoinTradeDataWithRandomVariation(_coinTradePoolDict[lootPoolId].GetRandomOutput(),
                randomVariationRange: 0.2f);
        }

        #endregion

        #region RecipeRandomPool

        public static RandomPool<RecipeGenerateInfo, RecipeInfo> CreateRelatedRecipePool(HashSet<PlantId> plantIds)
        {
            var relatedRecipes = new List<RecipeGenerateInfo>();
            foreach (var recipeGenerateInfo in RecipeConfigReader.GetAllRecipeGenerateInfos())
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