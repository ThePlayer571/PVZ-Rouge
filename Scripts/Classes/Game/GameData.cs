using TPL.PVZR.Classes.GameStuff;
using TPL.PVZR.Classes.MazeMap;

namespace TPL.PVZR.Classes.Game
{
    public class GameData : IGameData
    {
        public IMazeMapData MazeMapData { get; set; }
        public InventoryData InventoryData { get; set; }
        public GlobalEntityData GlobalEntityData { get; set; }


        //

        public GameData(MazeMapData mazeMapData, InventoryData inventoryData)
        {
            this.MazeMapData = mazeMapData;
            this.InventoryData = inventoryData;
            this.GlobalEntityData = new GlobalEntityData();
        }
    }
}