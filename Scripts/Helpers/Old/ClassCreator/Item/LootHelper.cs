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
                    return new LootData(LootType.Card, plantId: lootInfo.PlantId);
                case LootType.PlantBook:
                    return new LootData(LootType.PlantBook, plantBookId: lootInfo.PlantBookId);
                case LootType.Coin:
                    return new LootData(LootType.Coin, coinAmount: lootInfo.CoinAmount);
            }

            throw new ArgumentException($"未考虑的lootType: {lootInfo.LootType}");
        }
    }
}