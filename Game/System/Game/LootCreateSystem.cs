using System;
using System.Collections.Generic;
using QFramework;

namespace TPL.PVZR
{

    public interface ILootCreateSystem:ISystem
    {
        public List<LootData> CreateSetOfLootData(float value, List<LootData> lootDataList);
    }
    public class LootCreateSystem:AbstractSystem,ILootCreateSystem
    {
        protected override void OnInit()
        {
            throw new System.NotImplementedException();
        }

        // TODO: 参数太多了，以后写一个类统一存储
        public List<LootData> CreateSetOfLootData(float value, System.Tuple<float,float> countRange,List<LootData> lootDataList)
        {
            List<LootData> result = new();
            return result;
        }
    }
}