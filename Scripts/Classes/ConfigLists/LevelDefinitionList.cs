using System;
using System.Collections.Generic;
using TPL.PVZR.Classes.DataClasses.Level;
using UnityEngine;

namespace TPL.PVZR.Classes.ConfigLists
{
    [Serializable]
    public class LevelConfig
    {
        public LevelId levelId;
        public LevelDefinition levelDefinition;
    }
    
    [CreateAssetMenu(fileName = "LevelDefinitionList", menuName = "PVZR_Config/LevelDefinitionList")]
    public class LevelDefinitionList : ScriptableObject
    {
        public List<LevelConfig> levelDefinitionList;
    }
}