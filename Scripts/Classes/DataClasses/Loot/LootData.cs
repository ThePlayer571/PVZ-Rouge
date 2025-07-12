using TPL.PVZR.Classes.DataClasses.Item.Card;
using UnityEngine;

namespace TPL.PVZR.Classes.DataClasses.Loot
{
    public class LootData
    {
        public LootType LootType { get; }
        public CardData CardData { get; }

        public LootData(LootType lootType, CardData cardData = null)
        {
            LootType = lootType;
            CardData = cardData;
        }
    }
}