using System;
using System.Collections.Generic;
using System.Linq;
using TPL.PVZR.Classes.MazeMap.Controllers;
using TPL.PVZR.Tools;

namespace TPL.PVZR.Classes.MazeMap
{
    public interface IMazeMapController
    {
        #region Matrix生成

        void ValidateMazeMapData();
        void GenerateMazeMatrix();

        #endregion

        void SetMazeMapTiles();
    }


    public abstract class MazeMapController : IMazeMapController
    {
        protected MazeMapData MazeMapData { get; }

        protected Matrix<Node> mazeMatrix { get; set; }
        protected Dictionary<Node, List<Node>> adjacencyList { get; set; }
        protected Node startNode => mazeMatrix.First(n => n.level == 0);


        public abstract void ValidateMazeMapData();
        public abstract void GenerateMazeMatrix();

        public abstract void SetMazeMapTiles();


        protected MazeMapController(MazeMapData mazeMapData)
        {
            MazeMapData = mazeMapData;
        }

        public static IMazeMapController CreateController(MazeMapData mazeMapData)
        {
            return mazeMapData.Id switch
            {
                MazeMapId.DaveLawn => new DaveLawnController(mazeMapData),
                _ => throw new ArgumentException()
            };
        }
    }
}