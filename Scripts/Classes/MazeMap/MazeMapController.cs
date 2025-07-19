using System;
using System.Collections.Generic;
using System.Linq;
using QFramework;
using TPL.PVZR.Classes.DataClasses.Tomb;
using TPL.PVZR.Classes.MazeMap.Controllers;
using TPL.PVZR.Helpers.New.ClassCreator;
using TPL.PVZR.Tools;
using UnityEngine;

namespace TPL.PVZR.Classes.MazeMap
{
    public interface IMazeMapController
    {
        #region 数据：Matrix

        void GenerateMazeMatrix();

        #endregion

        #region 数据：Tomb

        void InitializeMazeMapData();
        void BreakTomb(ITombData tombData);

        #endregion

        void SetUpView();
    }


    public abstract class MazeMapController : IMazeMapController
    {
        #region 字段

        protected IMazeMapWiseData MazeMapData { get; }

        protected Matrix<Node> mazeMatrix { get; set; }

        protected Dictionary<Node, List<Node>> adjacencyList { get; set; }

        private List<Node> GetKeyAdjacentNodes(Node node)
        {
            if (!node.isKey) throw new ArgumentException();
            var result = new List<Node>();
            var x = node.x + 2;
            foreach (var y in Enumerable.Range(0, MazeMapData.ColCount))
            {
                var adjacentNode = mazeMatrix[x, y];

                if (adjacentNode != null && adjacentNode.isKey)
                {
                    result.Add(adjacentNode);
                }
            }

            if (result.Count == 0) throw new Exception();
            return result;
        }

        protected Node startNode => mazeMatrix.First(n => n.level == 0);

        #endregion

        #region 数据：Matrix

        public abstract void ValidateMazeMapData();
        public abstract void GenerateMazeMatrix();

        #endregion

        #region 数据：Tomb

        public void InitializeMazeMapData()
        {
            if (mazeMatrix == null) throw new Exception("MazeMatrix尚未生成，请先调用GenerateMazeMatrix()方法");

            foreach (var node in GetKeyAdjacentNodes(startNode))
            {
                // $"find: {startNode.Position}->{node.Position}".LogInfo();
                var chosenLevelDef = MazeMapData.GetRandomLevelOfStage(node.level);
                var tombData = TombCreator.CreateTombData(new Vector2Int(node.x, node.y), chosenLevelDef);
                MazeMapData.AddDiscoveredTomb(tombData);
            }
        }

        public void BreakTomb(ITombData tombData)
        {
            if (mazeMatrix == null) throw new Exception("MazeMatrix尚未生成，请先调用GenerateMazeMatrix()方法");

            MazeMapData.AddPassedTomb(tombData);
            var current = mazeMatrix[tombData.Position.x, tombData.Position.y];

            foreach (var toDiscover in GetKeyAdjacentNodes(current))
            {
                var chosenLevelDef = MazeMapData.GetRandomLevelOfStage(toDiscover.level);
                var data = TombCreator.CreateTombData(new Vector2Int(toDiscover.x, toDiscover.y), chosenLevelDef);
                MazeMapData.AddDiscoveredTomb(data);
            }
        }

        #endregion

        #region View

        public void SetUpView()
        {
            SetUpTiles();
            SetUpTombs();
        }

        protected abstract void SetUpTiles();
        protected abstract void SetUpTombs();

        #endregion

        protected MazeMapController(IMazeMapData mazeMapData)
        {
            MazeMapData = mazeMapData as IMazeMapWiseData;
        }

        public static IMazeMapController Create(IMazeMapData mazeMapData)
        {
            return mazeMapData.Def.Id switch
            {
                MazeMapId.DaveLawn => new DaveLawnController(mazeMapData),
                _ => throw new ArgumentException()
            };
        }
    }
}