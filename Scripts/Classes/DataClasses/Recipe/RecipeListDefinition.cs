using System.Collections.Generic;
using UnityEngine;

namespace TPL.PVZR.Classes.DataClasses.Recipe
{
    [CreateAssetMenu(fileName = "RecipeListDefinition", menuName = "PVZR/RecipeListDefinition", order = 5)]
    public class RecipeListDefinition : ScriptableObject
    {
        public List<RecipeGenerateInfo> recipes;
    }
}