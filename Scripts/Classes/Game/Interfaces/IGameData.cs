using TPL.PVZR.Classes.MazeMap;

namespace TPL.PVZR.Classes.Game
{
    public interface IGameData
    {
        IMazeMapData MazeMapData { get; set; }

    }
}