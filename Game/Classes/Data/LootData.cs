using System.Collections;
using System.Collections.Generic;
using TPL.PVZR.EntityZombie;

namespace TPL.PVZR
{
    
    public partial class LootData
    {
        public LootType lootType;
        public CardDataSO cardData;
        public float weight;
        public float value;
    }

    public partial class LootData
    {
        public enum LootType
        {
            Card
        }
        
    }
}