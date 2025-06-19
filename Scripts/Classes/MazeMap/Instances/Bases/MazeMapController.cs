using System;
using QFramework;
using TPL.PVZR.Classes.MazeMap.Generator;
using TPL.PVZR.Classes.MazeMap.Instances.DaveHouse;
using TPL.PVZR.Classes.MazeMap.Public;
using TPL.PVZR.Core;
using TPL.PVZR.Models;
using UnityEngine;

namespace TPL.PVZR.Classes.MazeMap
{
    public abstract class MazeMapController : IMazeMapController
    {
        #region Public

        public MazeMapData MazeMapData { get; protected set; }

        public abstract void SetMazeMapTiles();
        
        #endregion

        #region Private

        protected IPhaseModel _PhaseModel;

        #endregion


        protected MazeMapController(MazeMapData mazeMapData)
        {
            _PhaseModel = this.GetModel<IPhaseModel>();

            this.MazeMapData = mazeMapData;

            // 生成迷宫地图数据
            var generator = MazeMapGenerateHelper.GetGenerator(mazeMapData);
            (MazeMapData.mazeMatrix, MazeMapData.adjacencyList) = generator.Generate();
        }

        public IArchitecture GetArchitecture()
        {
            return PVZRouge.Interface;
        }
    }
}