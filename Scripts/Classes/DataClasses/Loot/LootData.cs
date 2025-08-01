using System;
using TPL.PVZR.Classes.DataClasses.Award;
using TPL.PVZR.Classes.DataClasses.Item.Card;
using TPL.PVZR.Classes.DataClasses.Item.PlantBook;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.Tools.Save;
using UnityEngine;

namespace TPL.PVZR.Classes.DataClasses.Loot
{
    public class LootData
    {
        public LootType LootType { get; }

        // Type: Card (此时未确定具体的变种，因此只存储PlantId)
        public PlantId PlantId { get; }
        public bool Locked { get; }

        // Type: PlantBook
        public PlantBookId PlantBookId { get; }

        // Type: Coin
        public int CoinAmount { get; }

        private LootData(LootType lootType, PlantId plantId = PlantId.NotSet, bool locked = false,
            PlantBookId plantBookId = PlantBookId.NotSet, int coinAmount = 0)
        {
            LootType = lootType;
            PlantId = plantId;
            Locked = locked;
            PlantBookId = plantBookId;
            CoinAmount = coinAmount;
        }

        public static LootData Create(LootInfo lootInfo)
        {
            switch (lootInfo.lootType)
            {
                case LootType.Card:
                    return new LootData(LootType.Card, plantId: lootInfo.plantId, locked: lootInfo.locked);
                case LootType.PlantBook:
                    return new LootData(LootType.PlantBook, plantBookId: lootInfo.plantBookId);
                case LootType.Coin:
                    return new LootData(LootType.Coin, coinAmount: lootInfo.coinAmount);
                case LootType.SeedSlot:
                    return new LootData(LootType.SeedSlot);
            }

            throw new ArgumentException($"未考虑的lootType: {lootInfo.lootType}");
        }
    }
}