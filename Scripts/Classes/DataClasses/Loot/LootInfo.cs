using System;
using PVZR.Tools;
using TPL.PVZR.Classes.DataClasses.Item.PlantBook;
using TPL.PVZR.Classes.InfoClasses;

namespace TPL.PVZR.Classes.DataClasses.Loot
{
    [Serializable]
    public class LootInfo
    {
        public LootType lootType;

        // Type: Card
        [EnumPaging(20)] public PlantId plantId;
        public bool locked = false;

        // Type: PlantBook
        public PlantBookId plantBookId;

        // Type: Coin
        public int coinAmount;
    }
}