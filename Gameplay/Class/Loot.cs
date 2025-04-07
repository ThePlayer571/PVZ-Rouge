using System.Collections.Generic;
using Newtonsoft.Json;
using QFramework;
using TPL.PVZR.Gameplay.Class.Items;
using TPL.PVZR.Gameplay.Data;
using TPL.PVZR.Gameplay.Entities;
using UnityEngine;

namespace TPL.PVZR.Gameplay.Class
{
    // 数据
    public class Loot
    {
        # region 属性
        public LootType lootType;
        public CardSO cardSO;
        public float weight;
        public float value;
        public enum LootType
        {
            Card
        }
        # endregion
        
        # region 静态方法
        
        public static Loot GetDefaultData(PlantIdentifier plantIdentifier)
        {
            return CardLootDefaultDataDict.TryGetValue(plantIdentifier, out var data)
                ? data
                : CardLootDefaultDataDict[PlantIdentifier.PeaShooter];
        }
        # endregion
        
        # region 私有
        private static readonly Dictionary<PlantIdentifier, Loot> CardLootDefaultDataDict = new();

        static Loot()
        {
            var json = ResLoader.Allocate().LoadSync<TextAsset>("CardLootDataJson");
            var CardLootDataJsonList = JsonConvert.DeserializeObject<List<CardLootDataJson>>(json.text);
            
            foreach (var each in CardLootDataJsonList)
            {
                CardLootDefaultDataDict[each.id] = new Loot
                {
                    lootType = LootType.Card,
                    cardSO = Card.GetCardSO(each.id),
                    weight = each.weight,
                    value = each.value
                };
            }
        }

        internal struct CardLootDataJson
        {
            public PlantIdentifier id;
            public float weight;
            public float value;
        }

        # endregion
    }

}