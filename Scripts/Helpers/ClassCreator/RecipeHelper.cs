using System.Collections.Generic;
using QAssetBundle;
using QFramework;
using TPL.PVZR.Classes;
using TPL.PVZR.Classes.DataClasses.Recipe;
using TPL.PVZR.Helpers.ClassCreator.Item;
using TPL.PVZR.Tools;
using TPL.PVZR.Tools.Random;

namespace TPL.PVZR.Helpers.ClassCreator
{
    public static class RecipeHelper
    {
        static RecipeHelper()
        {
            var resLoader = ResLoader.Allocate();
            Default = resLoader.LoadSync<RecipeListDefinition>(Recipelistdefinition.BundleName,
                Recipelistdefinition.RecipeListDefinition_Default);
        }

        private static RecipeListDefinition Default;

        public static RandomPool<RecipeGenerateInfo, RecipeInfo> GetRelatedRecipes(HashSet<PlantId> plantIds)
        {
            var relatedRecipes = new List<RecipeGenerateInfo>();
            foreach (var recipeGenerateInfo in Default.recipes)
            {
                foreach (var recipePlantId in recipeGenerateInfo.recipeInfo.ingredients.cards)
                {
                    if (plantIds.Contains(recipePlantId))
                    {
                        relatedRecipes.Add(recipeGenerateInfo);
                        break;
                    }
                }
            }

            return new RandomPool<RecipeGenerateInfo, RecipeInfo>(relatedRecipes, 1, RandomHelper.Game);
        }
    }
}