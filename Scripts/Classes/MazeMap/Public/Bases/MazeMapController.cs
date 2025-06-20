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

        public IMazeMapData MazeMapData { get; protected set; }

        public abstract void SetMazeMapTiles();
        
        #endregion

        
        
        
        protected IPhaseModel _PhaseModel;
        
        protected MazeMapController(IMazeMapData mazeMapData)
        {
            _PhaseModel = this.GetModel<IPhaseModel>();

            this.MazeMapData = mazeMapData;
        }

        public IArchitecture GetArchitecture()
        {
            return PVZRouge.Interface;
        }
    }
}