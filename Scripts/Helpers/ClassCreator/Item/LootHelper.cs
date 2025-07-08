using TPL.PVZR.Classes.DataClasses.Card;
using TPL.PVZR.Classes.DataClasses.Loot;

namespace TPL.PVZR.Helpers.ClassCreator
{
    public static class LootHelper
    {
        public static LootData CreateLootData(CardData cardData)
        {
            return new LootData(LootType.Card) { CardData = cardData };
        }
    }
}