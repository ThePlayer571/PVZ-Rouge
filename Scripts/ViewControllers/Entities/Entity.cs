using QFramework;
using TPL.PVZR.Classes.GameStuff;
using TPL.PVZR.Classes.Level;
using TPL.PVZR.Models;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Entities
{
    public class Entity : MonoBehaviour, IController
    {
        protected ILevelModel _LevelModel { get; private set; }
        protected ILevelData LevelData => _LevelModel.LevelData;
        protected IPhaseModel _PhaseModel { get; private set; }
        protected GlobalEntityData GlobalEntityData => _LevelModel.LevelData.GlobalEntityData;

        #region Unity生命周期
        protected virtual void Awake()
        {
            _LevelModel = this.GetModel<ILevelModel>();
            _PhaseModel = this.GetModel<IPhaseModel>();
        }

        protected virtual void Update()
        {
            
        }
        #endregion

        public IArchitecture GetArchitecture()
        {
            return PVZRouge.Interface;
        }
    }
}