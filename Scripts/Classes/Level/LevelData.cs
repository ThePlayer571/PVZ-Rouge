using TPL.PVZR.Classes.Game;
using TPL.PVZR.Classes.GameStuff;
using UnityEngine;

namespace TPL.PVZR.Classes.Level
{
    public class LevelData : ILevelData
    {
        // Runtime Definition
        public int InitialSunPoint { get; }
        public GlobalEntityData GlobalEntityData { get; }

        // Definition
        public LevelDefinition LevelDefinition { get; }
        


        public LevelData(in IGameData gameData, in LevelDefinition levelDefinition)
        {
            this.InitialSunPoint = gameData.InventoryData.InitialSunPoint;
            this.GlobalEntityData = gameData.GlobalEntityData;
            
            this.LevelDefinition = levelDefinition;
        }
    }
}