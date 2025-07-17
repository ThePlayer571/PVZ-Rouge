using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using QAssetBundle;
using QFramework;
using TPL.PVZR.Classes;
using TPL.PVZR.Classes.ConfigLists;
using TPL.PVZR.Classes.DataClasses;
using TPL.PVZR.Classes.DataClasses.Item.PlantBook;
using TPL.PVZR.Classes.DataClasses.Loot;
using TPL.PVZR.Classes.DataClasses.Recipe;
using TPL.PVZR.Classes.InfoClasses;
using UnityEngine;

namespace TPL.PVZR.Helpers.New.DataReader
{
    public static class TradeConfigReader
    {
        #region 数据配置

        private static readonly Dictionary<PlantId, int> _plantStandardValue = new();

        private static readonly Dictionary<PlantBookId, int> _plantBookStandardValue = new();

        private static readonly List<RecipeGenerateInfo> _recipes = new();

        static TradeConfigReader()
        {
            ResKit.Init();
            var resLoader = ResLoader.Allocate();
            var itemValueList = resLoader.LoadSync<ItemValueList>(Configlist.BundleName, Configlist.ItemValueList);

            // 设置PlantStandardValue
            var plantStandardValueJson = itemValueList.plantValueListJson[0].text;
            JObject plantValueList = JObject.Parse(plantStandardValueJson);
            foreach (var plantValueConfig in plantValueList)
            {
                if (Enum.TryParse<PlantId>(plantValueConfig.Key, out var plantId))
                {
                    _plantStandardValue.Add(plantId, plantValueConfig.Value.Value<int>());
                }
                else
                {
                    throw new Exception($"在文件中发现未定义的PlantId: {plantValueConfig.Key}");
                }
            }

            // 设置PlantBookStandardValue
            var plantBookStandardValueJson = itemValueList.plantBookValueListJson[0].text;
            JObject plantBookValueList = JObject.Parse(plantBookStandardValueJson);
            foreach (var plantBookValueConfig in plantBookValueList)
            {
                if (Enum.TryParse<PlantBookId>(plantBookValueConfig.Key, out var plantBookId))
                {
                    _plantBookStandardValue.Add(plantBookId, plantBookValueConfig.Value.Value<int>());
                }
                else
                {
                    throw new Exception($"在文件中发现未定义的PlantBookId: {plantBookValueConfig.Key}");
                }
            }

            // 配置Recipes
            var recipeConfigJson = resLoader
                .LoadSync<RecipeConfigList>(Configlist.BundleName, Configlist.RecipeConfigList).recipeListJson[0].text;
            JArray recipeConfigList = JArray.Parse(recipeConfigJson);


            foreach (JObject recipeConfig in recipeConfigList)
            {
                var recipeGenerateInfo = ParseToSoyo(recipeConfig);
                _recipes.Add(recipeGenerateInfo);
            }

            return;

            RecipeGenerateInfo ParseToSoyo(JObject recipeConfig)
            {
                var weight = 0f;
                var outputCard = PlantId.NotSet;
                var outputPlantBook = PlantBookId.NotSet;
                var inputCards = new List<PlantId>();
                var inputCoinRange = Vector2Int.zero;

                #region set

                {
                    // weight
                    weight = recipeConfig.Value<float>("weight");

                    // outputCard
                    JToken outputCardToken = recipeConfig["output"]?["card"];
                    if (outputCardToken != null)
                    {
                        Enum.TryParse(outputCardToken.Value<string>(), out outputCard);
                    }

                    // outputPlantBook
                    JToken plantBookToken = recipeConfig["output"]?["plantBook"];
                    if (plantBookToken != null)
                    {
                        Enum.TryParse(plantBookToken.Value<string>(), out outputPlantBook);
                    }

                    // inputCards
                    JArray inputCardsArray = recipeConfig["input"]?["cards"] as JArray;
                    if (inputCardsArray != null)
                    {
                        foreach (var c in inputCardsArray)
                        {
                            PlantId pid = PlantId.NotSet;
                            Enum.TryParse(c.Value<string>(), out pid);
                            inputCards.Add(pid);
                        }
                    }

                    // inputCoinRange
                    JArray coinRangeArray = recipeConfig["input"]?["coinRange"] as JArray;
                    if (coinRangeArray != null && coinRangeArray.Count == 2)
                    {
                        inputCoinRange = new Vector2Int(
                            coinRangeArray[0].Value<int>(),
                            coinRangeArray[1].Value<int>());
                    }
                }

                #endregion

                var lootInfo = outputCard != PlantId.NotSet
                    ? new LootInfo { lootType = LootType.Card, plantId = outputCard }
                    : new LootInfo { lootType = LootType.PlantBook, plantBookId = outputPlantBook };
                return new RecipeGenerateInfo()
                {
                    weight = weight,
                    recipeInfo = new RecipeInfo()
                    {
                        inputCards = inputCards,
                        inputCoinRange = inputCoinRange,
                        output = lootInfo
                    }
                };
            }
        }

        #endregion

        #region 数据读取

        public static int GetValueOf(PlantId plantId)
        {
            if (_plantStandardValue.TryGetValue(plantId, out var value))
            {
                return value;
            }

            throw new ArgumentException($"未找到PlantId: {plantId}，请检查配置文件");
        }

        public static int GetValueOf(PlantBookId plantBookId)
        {
            if (_plantBookStandardValue.TryGetValue(plantBookId, out var value))
            {
                return value;
            }

            throw new ArgumentException($"未找到PlantBookId: {plantBookId}，请检查配置文件");
        }

        public static IReadOnlyList<RecipeGenerateInfo> GetAllRecipeGenerateInfos()
        {
            return _recipes;
        }

        public static IReadOnlyDictionary<PlantId, int> GetAllPlantStandardValues()
        {
            return _plantStandardValue;
        }

        public static IReadOnlyDictionary<PlantBookId, int> GetAllPlantBookStandardValues()
        {
            return _plantBookStandardValue;
        }

        #endregion
    }
}