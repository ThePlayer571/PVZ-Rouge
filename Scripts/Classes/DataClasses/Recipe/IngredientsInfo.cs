using System;
using System.Collections.Generic;
using UnityEngine;

namespace TPL.PVZR.Classes.DataClasses.Recipe
{
    [Serializable]
    public class IngredientsInfo
    {
        public List<PlantId> cards;
        [Min(0)] public Vector2Int coinRange;
    }
}