using System.Collections.Generic;
using TPL.PVZR.Classes.DataClasses.Card;

namespace TPL.PVZR.Classes.DataClasses
{
    
    
    public class InventoryData
    {
        // 玩家属性
        public int InitialSunPoint { get; set; } = 75;
        public int SeedSlotCount { get; set; } = 4;
        
        // 玩家卡牌
        public List<CardData> Cards { get; set; } = new List<CardData>();
    }
}