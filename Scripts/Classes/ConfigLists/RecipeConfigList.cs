using System.Collections.Generic;
using UnityEngine;

namespace TPL.PVZR.Classes.ConfigLists
{
    [CreateAssetMenu(fileName = "RecipeConfigList", menuName = "PVZR_Config/RecipeConfigList")]
    public class RecipeConfigList : ScriptableObject
    {
        public List<TextAsset> recipeListJson;
    }
}