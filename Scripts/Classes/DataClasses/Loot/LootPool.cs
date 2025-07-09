using System;
using System.Collections.Generic;
using System.Linq;
using TPL.PVZR.Tools.Random;

namespace TPL.PVZR.Classes.DataClasses.Loot
{
    public class LootPool
    {
        private List<LootGenerateInfo> infos;
        private float remainingValue;
        private float allowedValueOffset;

        public bool IsFinished { get; private set; }

        private LootInfo GetRandomLoot()
        {
            if (infos.Count == 0) return null;

            for (var tryCount = 0; tryCount < 100; tryCount++)
            {
                var totalWeight = infos.Sum(item => item.weight);
                var chosenWeight = RandomHelper.Default.Value * totalWeight;

                LootGenerateInfo chosenInfo = null;
                var currentWeight = 0f;
                foreach (var info in infos)
                {
                    currentWeight += info.weight;
                    if (currentWeight >= chosenWeight)
                    {
                        chosenInfo = info;
                        break;
                    }
                }

                if (chosenInfo == null)
                    throw new Exception($"ChosenInfo为null, totalWeight: {totalWeight}, chosenWeight: {chosenWeight}");

                var reasonable = remainingValue - chosenInfo.value > -allowedValueOffset;
                if (reasonable)
                {
                    remainingValue -= chosenInfo.value;
                    if (remainingValue < allowedValueOffset)
                    {
                        IsFinished = true;
                    }
                    return chosenInfo.lootInfo;
                }
            }

            IsFinished = true;
            return null;
        }

        public List<LootInfo> GetAllRemainingLoots()
        {
            List<LootInfo> results = new List<LootInfo>();
            
            while (!IsFinished)
            {
                var loot = GetRandomLoot();
                if (loot != null)
                {
                    results.Add(loot);
                }
                else
                {
                    break;
                }
            }

            return results;
        }

        public LootPool(List<LootGenerateInfo> infos, float value)
        {
            this.infos = new List<LootGenerateInfo>(infos);
            remainingValue = value;
            allowedValueOffset = value * 0.05f;
        }
    }
}