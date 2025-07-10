using System;
using TPL.PVZR.Classes.DataClasses.Loot;

namespace TPL.PVZR.Classes.DataClasses.Recipe
{
    [Serializable]
    public class RecipeInfo
    {
        public IngredientsInfo ingredients;
        public LootInfo output;
    }
}