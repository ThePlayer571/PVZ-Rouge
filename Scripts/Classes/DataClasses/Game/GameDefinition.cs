using System;
using System.Collections.Generic;
using TPL.PVZR.Classes.DataClasses.Loot;
using TPL.PVZR.Classes.MazeMap;
using UnityEngine;

namespace TPL.PVZR.Classes.DataClasses.Game
{
    [CreateAssetMenu(fileName = "GameDefinition_", menuName = "PVZR/GameDefinition", order = 2)]
    public class GameDefinition : ScriptableObject
    {
        public GameDef GameDef;
        public MazeMapDef MazeMapDef;
        public List<LootInfo> InventoryLoots;
    }
}