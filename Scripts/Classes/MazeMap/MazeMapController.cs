using System;
using System.Collections.Generic;
using System.Linq;
using QFramework;
using TPL.PVZR.Classes.DataClasses.Tomb;
using TPL.PVZR.Classes.MazeMap.Controllers;
using TPL.PVZR.Helpers.New.ClassCreator;
using TPL.PVZR.Tools;
using TPL.PVZR.Tools.Random;
using UnityEditor;
using UnityEngine;

namespace TPL.PVZR.Classes.MazeMap
{
    public interface IMazeMapController
    {
        #region 操作

        void GenerateMazeMatrix();
        void InitializeTombData();
        void BreakTomb(ITombData tombData);
        void SetUpView();

        #endregion

        #region 数据获取

        Vector2Int MatrixToTilemapPosition(Vector2Int matrixPos);

        /// <summary>
        /// tomb的”被探索“阶段
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        TombState GetTombState(Vector2Int position);

        ITombData GetTomb(Vector2Int position);
        public ITombData CurrentTomb { get; }
        public Vector2Int GetCurrentMatrixPos();

        #endregion
    }


    public abstract class MazeMapController : IMazeMapController
    {
        #region IMazeMapController接口实现

        #region 操作

        public abstract void GenerateMazeMatrix();

        public void InitializeTombData()
        {
            if (mazeMatrix == null) throw new Exception("MazeMatrix尚未生成，请先调用GenerateMazeMatrix()方法");

            // 把keyAdjacencyList的所有信息打印出来
            foreach (var kvp in keyAdjacencyList)
            {
                var node = kvp.Key;
                var adjacentNodes = kvp.Value;
            }

            foreach (var node in keyAdjacencyList[startNode])
            {
                // $"find: {startNode.Position}->{node.Position}".LogInfo();
                var chosenLevelDef = MazeMapData.GetRandomLevelOfStage(node.level);
                var tombData = TombCreator.CreateTombData(new Vector2Int(node.x, node.y), chosenLevelDef);
                MazeMapData.AddDiscoveredTomb(tombData);
                ActiveTombs.Add(tombData);
            }
        }

        public void BreakTomb(ITombData tombData)
        {
            if (mazeMatrix == null) throw new Exception("MazeMatrix尚未生成，请先调用GenerateMazeMatrix()方法");

            MazeMapData.AddPassedTomb(tombData);
            PassedTombs.Add(tombData);
            FormlyDiscoveredTombs.AddRange(ActiveTombs);
            ActiveTombs.Clear();
            var current = mazeMatrix[tombData.Position.x, tombData.Position.y];

            foreach (var toDiscover in keyAdjacencyList[current])
            {
                var chosenLevelDef = MazeMapData.GetRandomLevelOfStage(toDiscover.level);
                var data = TombCreator.CreateTombData(new Vector2Int(toDiscover.x, toDiscover.y), chosenLevelDef);
                MazeMapData.AddDiscoveredTomb(data);
                ActiveTombs.Add(data);
            }
        }

        public void SetUpView()
        {
            SetUpTiles();
            SetUpTombs();
        }

        #endregion

        #region 数据获取

        public TombState GetTombState(Vector2Int position)
        {
            // 实践：不能将State存在ITombData中，因为有些墓碑未生成TombData，这样NotDiscovered的tomb难办

            if (ActiveTombs.Any(t => t.Position == position)) return TombState.Active;
            if (CurrentTomb != null && CurrentTomb.Position == position) return TombState.Current;
            if (PassedTombs.Any(t => t.Position == position)) return TombState.Passed;
            if (FormlyDiscoveredTombs.Any(t => t.Position == position)) return TombState.FormlyDiscovered;

            return TombState.NotDiscovered;
        }

        public ITombData GetTomb(Vector2Int position)
        {
            foreach (var tomb in ActiveTombs)
            {
                if (tomb.Position == position) return tomb;
            }

            foreach (var tomb in PassedTombs)
            {
                if (tomb.Position == position) return tomb;
            }

            foreach (var tomb in FormlyDiscoveredTombs)
            {
                if (tomb.Position == position) return tomb;
            }

            return null;
        }

        public abstract Vector2Int MatrixToTilemapPosition(Vector2Int matrixPos);

        public ITombData CurrentTomb => PassedTombs.LastOrDefault();

        public Vector2Int GetCurrentMatrixPos()
        {
            if (CurrentTomb != null)
                return CurrentTomb.Position;
            else
                return startNode.Position;
        }

        #endregion

        #endregion

        #region 字段

        // 基本数据结构

        protected IMazeMapWiseData MazeMapData { get; }
        protected Matrix<Node> mazeMatrix { get; set; }
        protected Dictionary<Node, List<Node>> adjacencyList { get; set; }

        // 拓展数据结构（为了方便调用而设，可以由基本数据结构推出）
        protected Dictionary<Node, List<Node>> keyAdjacencyList { get; set; }
        protected Node startNode;
        private List<ITombData> FormlyDiscoveredTombs = new();
        private List<ITombData> PassedTombs = new();
        private List<ITombData> ActiveTombs = new();

        // 当前随机数
        protected DeterministicRandom Random { get; }

        #endregion

        #region overridable

        protected abstract void ValidateMazeMapData();
        protected abstract void SetUpTiles();
        protected abstract void SetUpTombs();

        #endregion

        #region 构造函数

        protected MazeMapController(IMazeMapData mazeMapData)
        {
            MazeMapData = mazeMapData as IMazeMapWiseData;
            Random = DeterministicRandom.Create(MazeMapData.GenerateSeed);
        }

        public static IMazeMapController Create(IMazeMapData mazeMapData)
        {
            return mazeMapData.Def.Id switch
            {
                MazeMapId.DaveLawn => new DaveLawnController(mazeMapData),
                _ => throw new ArgumentException()
            };
        }

        #endregion
    }

    public enum TombState
    {
        Active,
        Passed,
        Current,
        FormlyDiscovered,
        NotDiscovered
    }
}