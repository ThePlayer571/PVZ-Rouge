using System.Collections.Generic;
using QFramework;
using TPL.PVZR.Classes.DataClasses.Loot;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.Tools.Random;

namespace TPL.PVZR.Classes.DataClasses.Recipe
{
    public class RecipeData
    {
        public List<PlantId> consumeCards { get; }
        public int consumeCoins { get; }
        public LootData output { get; }
        public bool used { get; set; }

        public RecipeData(RecipeInfo info)
        {
            consumeCards = info.inputCards;
            consumeCoins = RandomHelper.Game.Range(info.inputCoinRange.x, info.inputCoinRange.y + 1);
            output = LootData.Create(info.output);
            used = false;
        }
    }
}