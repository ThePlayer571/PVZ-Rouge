using TPL.PVZR.Classes.DataClasses.Item.Card;
using UnityEngine;

namespace TPL.PVZR.Classes.DataClasses.Loot
{
    public class LootData
    {
        public LootType LootType { get; set; }
        public CardData CardData { get; set; }

        public LootData(LootType lootType)
        {
            LootType = lootType;
        }
    }
}