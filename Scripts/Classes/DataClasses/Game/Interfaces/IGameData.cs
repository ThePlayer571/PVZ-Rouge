using TPL.PVZR.Classes.MazeMap;

namespace TPL.PVZR.Classes.DataClasses.Game.Interfaces
{
    public interface IGameData
    {
        MazeMapData MazeMapData { get; set; }
        IInventoryData InventoryData { get; set; }
        GlobalEntityData GlobalEntityData { get; set; }
    }
}