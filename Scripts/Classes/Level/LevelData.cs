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
        public LevelId LevelId { get; } = LevelId.DaveLawn;
        public Vector2 InitialPlayerPos { get; } = new Vector2(20, 9);
        public Vector2Int MapSize { get; } = new Vector2Int(47, 37);


        public LevelData(in IGameData gameData)
        {
            this.InitialSunPoint = gameData.InventoryData.InitialSunPoint;
            this.GlobalEntityData = gameData.GlobalEntityData;
        }
    }
}