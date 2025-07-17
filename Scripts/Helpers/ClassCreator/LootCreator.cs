using TPL.PVZR.Classes.DataClasses.Item.Card;
using TPL.PVZR.Classes.DataClasses.Item.PlantBook;
using TPL.PVZR.Classes.DataClasses.Loot;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.Helpers.New.DataReader;

namespace TPL.PVZR.Helpers.New.ClassCreator
{
    public static class LootCreator
    {
        public static LootGenerateInfo CreateDefaultLootGenerateInfo(PlantId plantId, float? weight = null,
            float weightMultiplier = 1f)
        {
            var value = TradeConfigReader.GetValueOf(plantId);
            weight ??= PlantConfigReader.GetCardDefinition(plantId.ToDef()).Quality.ToWeight() * weightMultiplier;
            return new LootGenerateInfo
            {
                weight = weight.Value,
                value = value,
                lootInfo = CreateLootInfo(plantId)
            };
        }

        public static LootGenerateInfo CreateDefaultLootGenerateInfo(PlantBookId bookId, float? weight = null,
            float weightMultiplier = 1f)
        {
            var value = TradeConfigReader.GetValueOf(bookId);
            var definition = PlantBookConfigReader.GetPlantBookDefinition(bookId);
            weight ??= PlantConfigReader.GetCardDefinition(new PlantDef(definition.Id, definition.Variant)).Quality
                .ToWeight() * weightMultiplier;
            return new LootGenerateInfo
            {
                weight = weight.Value,
                value = value,
                lootInfo = CreateLootInfo(bookId)
            };
        }

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
    }
}