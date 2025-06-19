using QFramework;
using UnityEngine;

namespace TPL.PVZR.Classes.MazeMap
{
    public interface IMazeMapController : IController
    {
        MazeMapData MazeMapData { get; }
        void SetMazeMapTiles();
    }
}