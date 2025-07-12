using System;
using TPL.PVZR.Classes;
using TPL.PVZR.Classes.DataClasses.Loot;

namespace TPL.PVZR.Helpers.ClassCreator.Item
{
    public static class LootHelper
    {
        public static LootData CreateLootData(LootInfo lootInfo)
        {
            switch (lootInfo.LootType)
            {
                case LootType.Card:
                    var cardData = CardHelper.CreateCardData(PlantBookHelper.GetPlantDef(lootInfo.PlantId));
                    return new LootData(LootType.Card, cardData: cardData);
            }

            throw new ArgumentException($"未考虑的lootType: {lootInfo.LootType}");
        }
    }
}