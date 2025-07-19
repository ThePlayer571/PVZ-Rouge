using TPL.PVZR.Classes.MazeMap;

namespace TPL.PVZR.Classes.DataClasses.Game
{
    public interface IGameData
    {
        IMazeMapData MazeMapData { get; set; }
        IInventoryData InventoryData { get; set; }
        GlobalEntityData GlobalEntityData { get; set; }
    }

    public class GameData : IGameData
    {
        public IMazeMapData MazeMapData { get; set; }
        public IInventoryData InventoryData { get; set; }
        public GlobalEntityData GlobalEntityData { get; set; }


        //

        public GameData(IMazeMapData mazeMapData, IInventoryData inventoryData)
        {
            this.MazeMapData = mazeMapData;
            this.InventoryData = inventoryData;
            this.GlobalEntityData = new GlobalEntityData();
        }
    }
}