using TPL.PVZR.Classes.DataClasses;
using TPL.PVZR.Classes.DataClasses.Item.Card;

namespace TPL.PVZR.Helpers.ClassCreator
{
    public static class SeedHelper
    {
        public static SeedData CreateSeedData(int index, CardData cardData)
        {
            return new SeedData(index, cardData);
        }
    }
}