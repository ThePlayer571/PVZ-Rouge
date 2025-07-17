using System;
using TPL.PVZR.Classes.DataClasses.Item.PlantBook;
using TPL.PVZR.Classes.InfoClasses;

namespace TPL.PVZR.Classes.DataClasses.Loot
{
    [Serializable]
    public class LootInfo
    {
        public LootType lootType;

        // Type: Card
        public PlantId plantId;
        
        // Type: PlantBook
        public PlantBookId plantBookId;
        
        // Type: Coin
        public int coinAmount;
    }
}