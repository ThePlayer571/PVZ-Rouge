using System;
using QFramework;
using TPL.PVZR.Models;
using UnityEngine;

namespace TPL.PVZR.Classes.MazeMap
{
    public class MapController : IController
    {
        #region Public

        public MazeMapData MazeMapData { get; private set; }

        public GameObject MazeMapGO
        {
            private set { _mazeMapGO = value; }
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

        private GameObject _mazeMapGO;

        private GameObject GenerateMazeMapGo()
        {
            throw new NotImplementedException();
        }

        #endregion


        public MapController()
        {
            _PhaseModel = this.GetModel<IPhaseModel>();
        }

        public IArchitecture GetArchitecture()
        {
            return PVZRouge.Interface;
        }
    }
}