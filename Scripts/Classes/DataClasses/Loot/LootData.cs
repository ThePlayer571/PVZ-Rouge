using TPL.PVZR.Classes.DataClasses.Item.Card;
using TPL.PVZR.Classes.DataClasses.Item.PlantBook;
using UnityEngine;

namespace TPL.PVZR.Classes.DataClasses.Loot
{
    public enum PlantBookId
    {
        NotSet = 0,
        MungBeanBook = 1,
    }

    public class LootData
    {
        public LootType LootType { get; }

        // Type: Card (此时未确定具体的变种，因此只存储PlantId)
        public PlantId PlantId { get; }

        // Type: PlantBook
        public PlantBookId PlantBookId { get; }

        // Type: Coin
        public int CoinAmount { get; }

        public LootData(LootType lootType, PlantId plantId = PlantId.NotSet,
            PlantBookId plantBookId = PlantBookId.NotSet, int coinAmount = 0)
        {
            LootType = lootType;
            PlantId = plantId;
            PlantBookId = plantBookId;
            CoinAmount = coinAmount;
        }
    }
}