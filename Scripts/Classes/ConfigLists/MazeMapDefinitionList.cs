using System;
using System.Collections.Generic;
using TPL.PVZR.Classes.MazeMap;
using UnityEngine;

namespace TPL.PVZR.Classes.ConfigLists
{
    [Serializable]
    public class MazeMapConfig
    {
        public MazeMapDef mazeMapDef;
        public MazeMapDefinition mazeMapDefinition;
    }
    
    [CreateAssetMenu(fileName = "MazeMapDefinitionList", menuName = "PVZR_Config/MazeMapDefinitionList")]
    public class MazeMapDefinitionList : ScriptableObject
    {
        public List<MazeMapConfig> mazeMapDefinitionList;
    }
}