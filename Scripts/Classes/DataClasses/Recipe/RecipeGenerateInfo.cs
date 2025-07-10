using System;
using TPL.PVZR.Tools;

namespace TPL.PVZR.Classes.DataClasses.Recipe
{
    [Serializable]
    public class RecipeGenerateInfo : IGenerateInfo<RecipeInfo>
    {
        public float weight;
        public RecipeInfo recipeInfo;
        public float Value => 0;
        public float Weight => weight;
        public RecipeInfo Output => recipeInfo;
    }
}