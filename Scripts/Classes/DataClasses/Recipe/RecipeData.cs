using System.Collections.Generic;
using QFramework;
using TPL.PVZR.Classes.DataClasses.Loot;
using TPL.PVZR.Helpers.ClassCreator.Item;
using TPL.PVZR.Tools.Random;

namespace TPL.PVZR.Classes.DataClasses.Recipe
{
    public class RecipeData
    {
        public List<PlantId> consumeCards { get; }
        public int consumeCoins { get; }
        public LootData output { get; }

        public RecipeData(RecipeInfo info)
        {
            consumeCards = info.ingredients.cards;
            consumeCoins = RandomHelper.Game.Range(info.ingredients.coinRange.x, info.ingredients.coinRange.y);
            output = LootHelper.CreateLootData(info.output);
        }
    }
}