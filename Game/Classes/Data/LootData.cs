using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using QFramework;
using TPL.PVZR.EntityZombie;
using UnityEngine;

namespace TPL.PVZR
{
    
    public partial class LootData
    {
        public LootType lootType;
        public CardDataSO cardData;
        public float weight;
        public float value;
    }

    public partial class LootData
    {
        
        
        
        
        
        
        
        
        
        public enum LootType
        {
            Card
        }
        
        // Dictionary
        private static Dictionary<PlantIdentifier, LootData> CardLootDefaultDataDict = new();

        // GetDefaultData
        public static LootData GetDefaultData(PlantIdentifier plantIdentifier)
        {
            return CardLootDefaultDataDict.TryGetValue(plantIdentifier, out var data)
                ? data
                : CardLootDefaultDataDict[PlantIdentifier.PeaShooter];
        }
        static LootData()
        {
            var json = ResLoader.Allocate().LoadSync<TextAsset>("CardLootDataJson");
            var CardLootDataJsonList = JsonConvert.DeserializeObject<List<CardLootDataJson>>(json.text);
            
            foreach (var each in CardLootDataJsonList)
            {
                CardLootDefaultDataDict[each.id] = new LootData
                {
                    lootType = LootType.Card,
                    cardData = CardData.GetCardData(each.id),
                    weight = each.weight,
                    value = each.value
                };
            }
        }

        private struct CardLootDataJson
        {
            public PlantIdentifier id;
            public float weight;
            public float value;
        }
           
    }
}