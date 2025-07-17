using System;
using System.Collections.Generic;
using TPL.PVZR.Classes.MazeMap;
using UnityEngine;

namespace TPL.PVZR.Classes.DataClasses.Recipe
{
    [Serializable]
    public class MazeMapConfig
    {
        public MazeMapId mazeMapId;
        public MazeMapDefinition mazeMapDefinition;
    }
    
    public class MazeMapDefinitionList : ScriptableObject
    {
        public List<MazeMapConfig> mazeMapDefinitionList;
    }
}