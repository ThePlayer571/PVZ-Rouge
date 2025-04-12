using System.Collections.Generic;
using QFramework;
using TPL.PVZR.Core;
using TPL.PVZR.Core.Save.Modules;
using UnityEngine;

namespace TPL.PVZR.Gameplay.Class.MazeMap.Core
{
    public interface IMazeMap
    {
        MapConfig MapConfig { get; }
        LevelConfig LevelConfig { get; }
        GameObject MazeMapGirdGO { get; }
        public GameObject GenerateMazeMapGO();
        public Node startNode { get; }
    }


    public abstract partial class MazeMap : IMazeMap
    {
        #region 公有

        // View
        public GameObject MazeMapGirdGO
        {
            get
            {
                if (_MazeMapGirdGO is null) GenerateMazeMapGO();
                return _MazeMapGirdGO;
            }
        }
        

        // 关卡配置
        public abstract MapConfig MapConfig { get; }
        public abstract LevelConfig LevelConfig { get; }
        //
        public Node startNode { get; protected set; }
        #endregion

        protected GameObject _MazeMapGirdGO = null;

        // View
        public abstract GameObject GenerateMazeMapGO();

        // 运行时数据
        protected Matrix<Node> mazeGrid { get; private set; }
        protected HashSet<Node> keyNodes = null;

        protected List<Node> passSpotNodes = null; // 按level升序排列

        // 持久化数据
        protected readonly MazeMapSaveData MazeMapSaveData;

        #region 构造

        protected MazeMap(MazeMapSaveData data)
        {
            MazeMapSaveData = data;
            CreateMain();
        }

        #endregion
    }
}