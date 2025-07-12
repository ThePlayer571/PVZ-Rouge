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
                    // TODO plantId
                    var cardData = CardHelper.CreateCardData(new PlantDef(lootInfo.PlantId, PlantVariant.V0));
                    return new LootData(LootType.Card, cardData: cardData);
            }

            throw new NotImplementedException($"未考虑的lootType: {lootInfo.LootType}");
        }
    }
}