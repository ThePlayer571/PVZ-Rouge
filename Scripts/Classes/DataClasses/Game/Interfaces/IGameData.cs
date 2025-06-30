using TPL.PVZR.Classes.MazeMap.Interfaces;

namespace TPL.PVZR.Classes.DataClasses.Game.Interfaces
{
    public interface IGameData
    {
        IMazeMapData MazeMapData { get; set; }
        InventoryData InventoryData { get; set; }
        GlobalEntityData GlobalEntityData { get; set; }
    }
}