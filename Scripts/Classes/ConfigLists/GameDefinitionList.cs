using System;
using System.Collections.Generic;
using TPL.PVZR.Classes.DataClasses.Game;
using UnityEngine;

namespace TPL.PVZR.Classes.ConfigLists
{
    [Serializable]
    public class GameConfig
    {
        public GameDef gameDef;
        public GameDefinition gameDefinition;
    }
    
    [CreateAssetMenu(fileName = "GameDefinitionList", menuName = "PVZR_Config/GameDefinitionList")]
    public class GameDefinitionList : ScriptableObject
    {
        public List<GameConfig> gameDefinitionList;
    }
}