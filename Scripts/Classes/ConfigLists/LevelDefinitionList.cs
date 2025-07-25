using System;
using System.Collections.Generic;
using TPL.PVZR.Classes.DataClasses.Level;
using UnityEngine;

namespace TPL.PVZR.Classes.ConfigLists
{
    [CreateAssetMenu(fileName = "LevelDefinitionList", menuName = "PVZR_Config/LevelDefinitionList")]
    public class LevelDefinitionList : ScriptableObject
    {
        public List<LevelDefinition> levelDefinitionList;
    }
}