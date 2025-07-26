using System;
using System.Collections.Generic;
using System.Linq;
using TPL.PVZR.Classes.DataClasses.Item.PlantBook;
using TPL.PVZR.Classes.DataClasses.Loot;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.Classes.LootPool;
using TPL.PVZR.Tools.Save;

namespace TPL.PVZR.Classes.DataClasses.Award
{
    public class AwardData
    {
        public AwardGenerateInfo AwardsToGenerate;


        public AwardData(AwardGenerateInfo awardsToGenerate)
        {
            AwardsToGenerate = awardsToGenerate;
        }
    }

    [Serializable]
    public class AwardGenerateInfo
    {
        public List<LootPoolDef> basicLoots;
        public List<LootGenerateInfo> specialLoots;
        public float totalValue;
        public int choiceCount;
    }
}