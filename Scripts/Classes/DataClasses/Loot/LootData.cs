using System;
using System.Collections.Generic;
using System.Linq;
using TPL.PVZR.Classes.DataClasses.Item.Card;
using UnityEngine;
using TPL.PVZR.Tools.Random;

namespace TPL.PVZR.Classes.DataClasses.Loot
{
    public enum LootType
    {
        Card
    }

    public class LootPool
    {
        private List<LootGenerateInfo> _pool;

        public List<LootInfo> GetRandomLoot(int count)
        {
            if (_pool == null || _pool.Count == 0)
                throw new InvalidOperationException("掉落池为空，无法生成掉落物。");

            float totalWeight = _pool.Sum(item => item.weight);
            if (totalWeight <= 0)
                throw new InvalidOperationException("掉落池的总权重必须大于零。");

            List<LootInfo> results = new List<LootInfo>();
            for (int i = 0; i < count; i++)
            {
                float randomValue = RandomHelper.Game.Range(0, totalWeight);
                float cumulativeWeight = 0;

                foreach (var loot in _pool)
                {
                    cumulativeWeight += loot.weight;
                    if (randomValue <= cumulativeWeight)
                    {
                        results.Add(loot.lootInfo);
                        break;
                    }
                }
            }

            return results;
        }

        public LootPool(List<LootGenerateInfo> pool)
        {
            _pool = pool;
        }
    }

    [Serializable]
    public class LootGenerateInfo
    {
        public float value;
        public float weight;
        public LootInfo lootInfo;
    }

    [Serializable]
    public class LootInfo
    {
        public LootType LootType;

        // Type: Card
        public PlantId PlantId;
    }

    public class LootData
    {
        public LootType LootType { get; set; }
        public CardData CardData { get; set; }

        public LootData(LootType lootType)
        {
            LootType = lootType;
        }
    }
}