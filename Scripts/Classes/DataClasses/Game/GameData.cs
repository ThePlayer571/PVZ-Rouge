using TPL.PVZR.Classes.DataClasses.Game.Interfaces;
using TPL.PVZR.Classes.MazeMap;

namespace TPL.PVZR.Classes.DataClasses.Game
{
    public class GameData : IGameData
    {
        public MazeMapData MazeMapData { get; set; }
        public IInventoryData InventoryData { get; set; }
        public GlobalEntityData GlobalEntityData { get; set; }


        //

        public GameData(MazeMapData mazeMapData, IInventoryData inventoryData)
        {
            this.MazeMapData = mazeMapData;
            this.InventoryData = inventoryData;
            this.GlobalEntityData = new GlobalEntityData();
        }
    }
}