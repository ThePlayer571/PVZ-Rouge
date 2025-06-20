using QFramework;
using UnityEngine;

namespace TPL.PVZR.Classes.MazeMap
{
    public interface IMazeMapController : IController
    {
        IMazeMapData MazeMapData { get; }
        void SetMazeMapTiles();
    }
}