using TPL.PVZR.Classes.GameStuff;
using TPL.PVZR.Classes.LevelStuff;

namespace TPL.PVZR.Helpers.Factory
{
    public static class SeedHelper
    {
        public static SeedData CreateSeedData(int index, CardData cardData)
        {
            return new SeedData(index, cardData);
        }
    }
}