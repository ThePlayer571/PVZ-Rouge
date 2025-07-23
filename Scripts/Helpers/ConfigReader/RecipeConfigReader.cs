using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using QAssetBundle;
using QFramework;
using TPL.PVZR.Classes.ConfigLists;
using TPL.PVZR.Classes.DataClasses.Item.PlantBook;
using TPL.PVZR.Classes.DataClasses.Loot;
using TPL.PVZR.Classes.DataClasses.Recipe;
using TPL.PVZR.Classes.InfoClasses;
using UnityEngine;

namespace TPL.PVZR.Helpers.New.DataReader
{
    public class RecipeConfigReader
    {
        static RecipeConfigReader()
        {
            var resLoader = ResLoader.Allocate();
            // 配置Recipes
            var recipeConfigJson =
                resLoader.LoadSync<TextAsset>(Jsonconfigs.BundleName, Jsonconfigs.RecipeConfigs).text;
            JArray recipeConfigList = JArray.Parse(recipeConfigJson);


            foreach (JObject recipeConfig in recipeConfigList)
            {
                var recipeGenerateInfo = ParseToSoyo(recipeConfig);
                _recipes.Add(recipeGenerateInfo);
            }

            resLoader.Recycle2Cache();

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

        private static readonly List<RecipeGenerateInfo> _recipes = new();

        /// <summary>
        /// 获取所有的RecipeGenerateInfo
        /// </summary>
        /// <returns></returns>
        public static IReadOnlyList<RecipeGenerateInfo> GetAllRecipeGenerateInfos()
        {
            return _recipes;
        }
    }
}