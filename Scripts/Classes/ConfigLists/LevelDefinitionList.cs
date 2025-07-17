using System;
using System.Collections.Generic;
using TPL.PVZR.Classes.DataClasses.Level;
using UnityEngine;

namespace TPL.PVZR.Classes.DataClasses.Recipe
{
    [Serializable]
    public class LevelConfig
    {
        public LevelId levelId;
        public LevelDefinition levelDefinition;
    }
    
    public class LevelDefinitionList : ScriptableObject
    {
        public List<LevelConfig> levelDefinitionList;
    }
}