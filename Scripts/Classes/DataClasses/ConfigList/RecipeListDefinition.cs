using System.Collections.Generic;
using UnityEngine;

namespace TPL.PVZR.Classes.DataClasses.Recipe
{
    [CreateAssetMenu(fileName = "RecipeListConfig", menuName = "PVZR_Config/RecipeListConfig", order = 5)]
    public class RecipeListConfig : ScriptableObject
    {
        public List<RecipeGenerateInfo> recipes;
    }
}