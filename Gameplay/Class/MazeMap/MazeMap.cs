using System;
using System.Collections.Generic;
using System.Linq;
using QFramework;
using TPL.PVZR.Core;
using UnityEngine;

namespace TPL.PVZR.Gameplay.Class.MazeMap
{
    public interface IMazeMap
    {
        MapConfig MapConfig { get; }
        LevelConfig LevelConfig { get; }
        GameObject GenerateMazeMapGO();
    }

    

    

    public abstract partial class MazeMap
    {
        public Matrix<Node> mazeGrid { get; private set; }
        protected HashSet<Node> keyNodes = null;
        public GameObject MazeMapGirdGO { get; protected set; }

        public abstract MapConfig MapConfig { get; }
        public abstract LevelConfig LevelConfig { get; }

        public abstract GameObject GenerateMazeMapGO();
    }
}