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
    public class MazeMapController : IController
    {
        #region Public

        public MazeMapData MazeMapData { get; private set; }

        public GameObject MazeMapGO
        {
            private set => _mazeMapGO = value;
            get
            {
                if (_PhaseModel.GamePhase != GamePhase.MazeMap)
                    throw new Exception($"在不正确的时机获取MazeMapGO：{_PhaseModel.GamePhase}");
                if (!_mazeMapGO) _mazeMapGO = GenerateMazeMapGo();
                return _mazeMapGO;
            }
        }

        #endregion

        #region Private

        private IPhaseModel _PhaseModel;

        // _mazeMapGO
        private GameObject _mazeMapGO;

        private GameObject GenerateMazeMapGo()
        {
            throw new NotImplementedException();
        }
        #endregion


        public MazeMapController(MazeMapData mazeMapData)
        {
            _PhaseModel = this.GetModel<IPhaseModel>();
            
            this.MazeMapData = mazeMapData;
            
            // 生成迷宫地图数据
            var generator = MazeMapGenerateHelper.GetGenerator(mazeMapData);
           (MazeMapData.mazeMatrix)= generator.Generate();
        }

        public IArchitecture GetArchitecture()
        {
            return PVZRouge.Interface;
        }
    }
}