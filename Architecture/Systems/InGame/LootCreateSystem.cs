using System;
using System.Collections.Generic;
using System.Linq;
using QFramework;
using TPL.PVZR.Core.Random;
using TPL.PVZR.Gameplay.Class;

namespace TPL.PVZR.Architecture.Systems.InGame
{

    public interface ILootCreateSystem:ISystem
    {
        public List<Loot> CreateSetOfLootData(float value,List<Loot> lootDataList);
    }
    public class LootCreateSystem:AbstractSystem,ILootCreateSystem
    {
        protected override void OnInit()
        {
            
        }

        // TODO: 参数太多了，以后写一个类统一存储 (3-19 删了一个参数)
        public List<Loot> CreateSetOfLootData(float value, List<Loot> lootDataList)
        {
            if (lootDataList.IsNullOrEmpty())
            {
                throw new System.NotImplementedException("啥b程序员，lootDataList是空的(或null)");
            }

            float totalWeight = lootDataList.Sum(each => each.weight);
            List<Loot> result = new();
            // 抽取result
            float tryCount = 0; // 尝试次数
            float cumulativeValue = 0; // 累计value
            while (cumulativeValue < value && tryCount++ < 100)
            {
                // 随机抽取一个loot
                Loot chosenLoot = null;
                {
                    float chosenWeight = RandomHelper.Game.Range(0, totalWeight);
                    float cumulativeWeight = 0;
                    foreach (var each in lootDataList)
                    {
                        cumulativeWeight += each.weight;
                        if (chosenWeight <= cumulativeWeight)
                        {
                            chosenLoot = each;
                            break;
                        }

                        // 防止出bug，给个默认值
                        chosenLoot = lootDataList[0];
                    }

                    // 防止出现过高的Value
                    if (chosenLoot == null)
                    {
                        throw new NotImplementedException("chosenLootData是空的");
                    }

                    if (cumulativeValue + chosenLoot.value > value * 1.5f) continue;
                    // 输出
                    cumulativeValue += chosenLoot.value;
                    result.Add(chosenLoot);
                }
            }

            return result;
        }
    }
}