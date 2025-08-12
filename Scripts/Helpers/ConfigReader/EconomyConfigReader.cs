using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using QAssetBundle;
using QFramework;
using TPL.PVZR.Classes;
using TPL.PVZR.Classes.ConfigLists;
using TPL.PVZR.Classes.DataClasses;
using TPL.PVZR.Classes.DataClasses.Item.Card;
using TPL.PVZR.Classes.DataClasses.Item.PlantBook;
using TPL.PVZR.Classes.DataClasses.Loot;
using TPL.PVZR.Classes.DataClasses.Recipe;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.Classes.LootPool;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace TPL.PVZR.Helpers.New.DataReader
{
    public static class EconomyConfigReader
    {
        #region 数据配置

        private static readonly Dictionary<PlantId, int> _plantStandardValue = new();

        private static readonly Dictionary<PlantBookId, int> _plantBookStandardValue = new();


        public static async Task InitializeAsync()
        {
            // 设置PlantStandardValue
            var plantStandardValueJsonHandle = Addressables.LoadAssetAsync<TextAsset>("PlantValueList");
            await plantStandardValueJsonHandle.Task;
            JObject plantValueList = JObject.Parse(plantStandardValueJsonHandle.Result.text);
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
            var plantBookStandardValueJsonHandle = Addressables.LoadAssetAsync<TextAsset>("PlantBookValueList");
            await plantBookStandardValueJsonHandle.Task;
            JObject plantBookValueList = JObject.Parse(plantBookStandardValueJsonHandle.Result.text);
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


            plantStandardValueJsonHandle.Release();
            plantBookStandardValueJsonHandle.Release();
        }

        #endregion

        #region 数据读取

        #region Weight

        public static float GetWeightOf(PlantBookId plantBookId)
        {
            var definition = PlantBookConfigReader.GetPlantBookDefinition(plantBookId);
            var cardDefinition =
                PlantConfigReader.GetCardDefinition(new PlantDef(definition.PlantId, definition.Variant));
            return GetWeightOf(cardDefinition.Quality) * 0.3f;
        }

        public static float GetWeightOf(int coinAmount)
        {
            return 10;
        }

        public static float GetWeightOf(CardQuality quality)
        {
            return quality switch
            {
                CardQuality.White => 20f,
                CardQuality.Green => 10f,
                CardQuality.Blue => 5f,
                CardQuality.Purple => 2f,
                CardQuality.Gold => 0f,
                _ => 0f
            };
        }

        public static float GetWeightOf(PlantId plantId)
        {
            var cardDefinition = PlantConfigReader.GetCardDefinition(plantId.ToDef());
            return GetWeightOf(cardDefinition.Quality);
        }


        public static float GetWeightOfSeedSlot()
        {
            return 1;
        }

        #endregion

        #region Value

        public static int GetValueOf(PlantId plantId)
        {
            if (_plantStandardValue.TryGetValue(plantId, out var value))
            {
                return value;
            }

            $"未找到PlantId: {plantId}，请检查配置文件".LogError();
            return 0;
        }

        public static int GetValueOf(PlantBookId plantBookId)
        {
            if (_plantBookStandardValue.TryGetValue(plantBookId, out var value))
            {
                return value;
            }

            throw new ArgumentException($"未找到PlantBookId: {plantBookId}，请检查配置文件");
        }

        public static int GetValueOf(int coinAmount)
        {
            return coinAmount;
        }


        public static int GetValueOfSeedSlot()
        {
            return 50;
        }

        #endregion

        /// <summary>
        /// 获取每个植物的“市场价”
        /// </summary>
        /// <returns></returns>
        public static IReadOnlyDictionary<PlantId, int> GetAllPlantStandardValues()
        {
            return _plantStandardValue;
        }

        /// <summary>
        ///  获取每个秘籍的“市场价”
        /// </summary>
        /// <returns></returns>
        public static IReadOnlyDictionary<PlantBookId, int> GetAllPlantBookStandardValues()
        {
            return _plantBookStandardValue;
        }

        #endregion
    }
}