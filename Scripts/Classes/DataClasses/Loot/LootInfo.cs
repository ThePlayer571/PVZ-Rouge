using System;

namespace TPL.PVZR.Classes.DataClasses.Loot
{
    [Serializable]
    public class LootInfo
    {
        public LootType LootType;

        // Type: Card
        public PlantId PlantId;
        
        // Type: PlantBook
        public PlantBookId PlantBookId;
        
        // Type: Coin
        public int CoinAmount;
    }
}