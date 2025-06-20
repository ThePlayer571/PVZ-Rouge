using TPL.PVZR.Classes.GameStuff;
using TPL.PVZR.Classes.MazeMap;

namespace TPL.PVZR.Classes.Game
{
    public interface IGameData
    {
        IMazeMapData MazeMapData { get; set; }
        InventoryData InventoryData { get; set; }
        GlobalEntityData GlobalEntityData { get; set; }
    }
}