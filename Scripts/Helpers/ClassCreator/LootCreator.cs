using System.Collections.Generic;
using System.Linq;
using TPL.PVZR.Classes.DataClasses.Item.Card;
using TPL.PVZR.Classes.DataClasses.Item.PlantBook;
using TPL.PVZR.Classes.DataClasses.Loot;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.Classes.LootPool;
using TPL.PVZR.Helpers.New.DataReader;
using TPL.PVZR.Tools.Random;

namespace TPL.PVZR.Helpers.New.ClassCreator
{
    public static class LootCreator
    {
        #region LootGenerateInfo

        /// <summary>
        /// 生成对应lootPool包含的所有loot的LootGenerateInfo列表
        /// </summary>
        /// <param name="lootPoolDef"></param>
        /// <returns></returns>
        public static List<LootGenerateInfo> CreateLootGenerateInfoList(LootPoolDef lootPoolDef)
        {
            var lootPool = LootPoolConfigReader.GetLootPoolInfo(lootPoolDef.Id);
            var lootGenerateInfos = new List<LootGenerateInfo>();
            lootGenerateInfos.AddRange(lootPool.cards.Select(plantId => CreateLootGenerateInfo(plantId)));
            lootGenerateInfos.AddRange(lootPool.plantBooks.Select(bookId => CreateLootGenerateInfo(bookId)));
            lootGenerateInfos.AddRange(lootPool.coinRanges.Select(vec2 =>
                CreateLootGenerateInfo(RandomHelper.Game.Range(vec2.x, vec2.y + 1))));
            if (lootPool.seedSlot)
                lootGenerateInfos.Add(CreateLootGenerateInfo_SeedSlot());
            return lootGenerateInfos;
        }

        /// <summary>
        /// 生成对应Card的LootGenerateInfo
        /// </summary>
        /// <param name="plantId"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public static LootGenerateInfo CreateLootGenerateInfo(PlantId plantId, float? weight = null)
        {
            var value = EconomyConfigReader.GetValueOf(plantId);
            weight ??= EconomyConfigReader.GetWeightOf(plantId);
            return new LootGenerateInfo
            {
                weight = weight.Value,
                value = value,
                lootInfo = CreateLootInfo(plantId)
            };
        }

        /// <summary>
        /// 生成对应PlantBook的LootGenerateInfo
        /// </summary>
        /// <param name="bookId"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public static LootGenerateInfo CreateLootGenerateInfo(PlantBookId bookId, float? weight = null)
        {
            var value = EconomyConfigReader.GetValueOf(bookId);
            var definition = PlantBookConfigReader.GetPlantBookDefinition(bookId);
            weight ??= EconomyConfigReader.GetWeightOf(bookId);
            return new LootGenerateInfo
            {
                weight = weight.Value,
                value = value,
                lootInfo = CreateLootInfo(bookId)
            };
        }

        /// <summary>
        ///  生成对应Coin的LootGenerateInfo
        /// </summary>
        /// <param name="coinAmount"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public static LootGenerateInfo CreateLootGenerateInfo(int coinAmount, float? weight = null)
        {
            var value = EconomyConfigReader.GetValueOf(coinAmount);
            weight ??= EconomyConfigReader.GetWeightOf(coinAmount);
            return new LootGenerateInfo
            {
                weight = weight.Value,
                value = value,
                lootInfo = CreateLootInfo(coinAmount)
            };
        }

        /// <summary>
        /// 生成SeedSlot的LootGenerateInfo
        /// </summary>
        /// <param name="weight"></param>
        /// <returns></returns>
        public static LootGenerateInfo CreateLootGenerateInfo_SeedSlot(float? weight = null)
        {
            var value = EconomyConfigReader.GetValueOfSeedSlot();
            weight ??= EconomyConfigReader.GetWeightOfSeedSlot();
            return new LootGenerateInfo
            {
                weight = weight.Value,
                value = value,
                lootInfo = CreateLootInfo_SeedSlot()
            };
        }

        #endregion

        #region LootInfo

        public static LootInfo CreateLootInfo(PlantId plantId)
        {
            return new LootInfo()
            {
                lootType = LootType.Card,
                plantId = plantId,
            };
        }

        public static LootInfo CreateLootInfo(PlantBookId bookId)
        {
            return new LootInfo()
            {
                lootType = LootType.PlantBook,
                plantBookId = bookId,
            };
        }

        public static LootInfo CreateLootInfo(int coinAmount)
        {
            return new LootInfo()
            {
                lootType = LootType.Coin,
                coinAmount = coinAmount,
            };
        }

        public static LootInfo CreateLootInfo_SeedSlot()
        {
            return new LootInfo
            {
                lootType = LootType.SeedSlot,
            };
        }

        #endregion
    }
}