using System;
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
                    var cardData = CardHelper.CreateCardData(lootInfo.PlantId);
                    return new LootData(LootType.Card) { CardData = cardData };
            }

            throw new NotImplementedException($"未考虑的lootType: {lootInfo.LootType}");
        }
    }
}