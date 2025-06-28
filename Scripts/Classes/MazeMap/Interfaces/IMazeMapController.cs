using QFramework;

namespace TPL.PVZR.Classes.MazeMap.Interfaces
{
    public interface IMazeMapController : IController
    {
        IMazeMapData MazeMapData { get; }
        void SetMazeMapTiles();
    }
}