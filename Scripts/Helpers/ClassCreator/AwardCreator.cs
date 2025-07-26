using System.Collections.Generic;
using System.Linq;
using TPL.PVZR.Classes.DataClasses.Award;
using TPL.PVZR.Classes.DataClasses.Item.PlantBook;
using TPL.PVZR.Classes.DataClasses.Loot;
using TPL.PVZR.Tools;
using TPL.PVZR.Tools.Random;

namespace TPL.PVZR.Helpers.New.ClassCreator
{
    public static class AwardCreator
    {
        public static List<List<LootData>> CreateAwardData(AwardGenerateInfo awardInfo,
            List<PlantBookData> excludePlantBooks = null)
        {
            excludePlantBooks ??= new List<PlantBookData>();
            var lootGenerateInfos = awardInfo.basicLoots
                .SelectMany(lootPoolDef => LootCreator.CreateLootGenerateInfoList(lootPoolDef)).ToList();
            lootGenerateInfos.AddRange(awardInfo.specialLoots);

            // exclude
            lootGenerateInfos = lootGenerateInfos.Where(info =>
                info.lootInfo.lootType != LootType.PlantBook ||
                excludePlantBooks.All(pb => pb.PlantId != info.lootInfo.plantId)).ToList();

            //
            var result = new List<List<LootData>>();
            for (int i = 0; i < awardInfo.choiceCount; i++)
            {
                var randomPool =
                    new RandomPool<LootGenerateInfo, LootInfo>(lootGenerateInfos, awardInfo.totalValue,
                        RandomHelper.Game);
                var chosenLoots = randomPool.GetAllRemainingOutputs()
                    .Select(lootInfo => LootData.Create(lootInfo));
                result.Add(chosenLoots.ToList());
            }

            return result;
        }
    }
}