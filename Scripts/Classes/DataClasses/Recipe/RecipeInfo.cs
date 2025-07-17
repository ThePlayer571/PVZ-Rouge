using System;
using System.Collections.Generic;
using TPL.PVZR.Classes.DataClasses.Loot;
using TPL.PVZR.Classes.InfoClasses;
using UnityEngine;

namespace TPL.PVZR.Classes.DataClasses.Recipe
{
    [Serializable]
    public class RecipeInfo
    {
        public List<PlantId> inputCards;
        [Min(0)] public Vector2Int inputCoinRange;
        public LootInfo output;
    }
}