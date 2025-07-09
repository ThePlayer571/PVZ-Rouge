using System;
using System.Collections.Generic;
using System.Linq;
using TPL.PVZR.Classes.MazeMap.New.Controllers;
using TPL.PVZR.Tools;

namespace TPL.PVZR.Classes.MazeMap.New
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
            switch (mazeMapData.Id)
            {
                case MazeMapId.DaveLawn:
                    return new DaveLawnController(mazeMapData);
            }

            throw new NotImplementedException();
        }
    }
}