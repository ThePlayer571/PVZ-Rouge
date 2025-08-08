using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using QAssetBundle;
using QFramework;
using TPL.PVZR.Classes.DataClasses.Item.PlantBook;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.Classes.LootPool;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace TPL.PVZR.Helpers.New.DataReader
{
    public static class LootPoolConfigReader
    {
        public static async Task InitializeAsync()
        {
            var lootPoolJsonHandle = Addressables.LoadAssetAsync<TextAsset>("LootPoolList");
            await lootPoolJsonHandle.Task;
            // 初始化_lootPoolDict，从JSON文件解析
            JArray lootPoolArray = JArray.Parse(lootPoolJsonHandle.Result.text);
            foreach (JObject lootPoolConfig in lootPoolArray)
            {
                var lootPoolInfo = new LootPoolInfo();

                // 解析weight
                lootPoolInfo.weight = lootPoolConfig.Value<float>("weight");

                // 解析lootPoolDef
                var lootPoolDefToken = lootPoolConfig["lootPoolDef"];
                if (lootPoolDefToken != null)
                {
                    var lootPoolIdStr = lootPoolDefToken.Value<string>("lootPoolId");
                    if (Enum.TryParse<LootPoolId>(lootPoolIdStr, out var lootPoolId))
                    {
                        lootPoolInfo.lootPoolDef = new LootPoolDef { Id = lootPoolId };
                    }
                }

                // 解析cards
                lootPoolInfo.cards = new List<PlantId>();
                var cardsArray = lootPoolConfig["cards"] as JArray;
                if (cardsArray != null)
                {
                    foreach (var cardToken in cardsArray)
                    {
                        var cardStr = cardToken.Value<string>();
                        if (Enum.TryParse<PlantId>(cardStr, out var plantId))
                        {
                            lootPoolInfo.cards.Add(plantId);
                        }
                    }
                }

                // 解析plantBooks
                lootPoolInfo.plantBooks = new List<PlantBookId>();
                var plantBooksArray = lootPoolConfig["plantBooks"] as JArray;
                if (plantBooksArray != null)
                {
                    foreach (var plantBookToken in plantBooksArray)
                    {
                        var plantBookStr = plantBookToken.Value<string>();
                        if (Enum.TryParse<PlantBookId>(plantBookStr, out var plantBookId))
                        {
                            lootPoolInfo.plantBooks.Add(plantBookId);
                        }
                    }
                }

                // 解析coinRanges
                lootPoolInfo.coinRanges = new List<Vector2Int>();
                var coinRangesArray = lootPoolConfig["coinRanges"] as JArray;
                if (coinRangesArray != null)
                {
                    foreach (JArray rangeArray in coinRangesArray)
                    {
                        if (rangeArray.Count == 2)
                        {
                            var coinRange = new Vector2Int(
                                rangeArray[0].Value<int>(),
                                rangeArray[1].Value<int>()
                            );
                            lootPoolInfo.coinRanges.Add(coinRange);
                        }
                    }
                }

                // 解析seedSlot
                lootPoolInfo.seedSlot = lootPoolConfig.Value<bool>("seedSlot");

                // 添加到字典中
                if (lootPoolInfo.lootPoolDef.Id != LootPoolId.NotSet)
                {
                    _lootPoolDict[lootPoolInfo.lootPoolDef.Id] = lootPoolInfo;
                }
            }
            
            //
            lootPoolJsonHandle.Release();
        }

        private static readonly Dictionary<LootPoolId, LootPoolInfo> _lootPoolDict = new();

        public static LootPoolInfo GetLootPoolInfo(LootPoolId lootPoolId)
        {
            if (_lootPoolDict.TryGetValue(lootPoolId, out var lootPoolInfo))
            {
                return lootPoolInfo;
            }

            throw new ArgumentException($"未找到LootPoolId: {lootPoolId}，请检查配置文件");
        }
    }
}