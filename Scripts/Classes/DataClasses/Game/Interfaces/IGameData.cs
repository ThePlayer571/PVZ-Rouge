using TPL.PVZR.Classes.MazeMap.New;

namespace TPL.PVZR.Classes.DataClasses.Game.Interfaces
{
    public interface IGameData
    {
        MazeMapData MazeMapData { get; set; }
        InventoryData InventoryData { get; set; }
        GlobalEntityData GlobalEntityData { get; set; }
    }
}