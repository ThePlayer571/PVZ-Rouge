using QFramework;
using TPL.PVZR.CommandEvents.Phase;
using TPL.PVZR.Helpers;
using TPL.PVZR.Helpers.Methods;
using TPL.PVZR.Models;
using TPL.PVZR.Tools;
using TPL.PVZR.ViewControllers.Entities.EntityBase;
using TPL.PVZR.ViewControllers.Managers;
using UnityEngine;

namespace TPL.PVZR.Systems
{
    public interface IEnvironmentSystem : ISystem
    {
        
    }

    
    public class EnvironmentSystem: AbstractSystem, IEnvironmentSystem
    {

        private void StartRunning()
        {
            _sunTimer.SetRemaining(5f);
            
            GameManager.ExecuteOnUpdate(Update);
        }

        private void StopRunning()
        {
            GameManager.StopOnUpdate(Update);
        }

        private Timer _sunTimer;
        private void Update()
        {
            _sunTimer.Update(Time.deltaTime);

            if (_sunTimer.Ready)
            {
                var pos = LevelGridHelper.CellToWorld(_LevelModel.LevelData.GetRandomSunFallCellPos());
                EntityFactory.SunFactory.SpawnSunWithFall(pos);
                _sunTimer.Reset();
            }
        }
        
        
        private ILevelModel _LevelModel;
        
        protected override void OnInit()
        {
            _LevelModel = this.GetModel<ILevelModel>();
            
            _sunTimer = new Timer(10f);
            
            this.RegisterEvent<OnPhaseChangeEvent>(e =>
            {
                switch (e.GamePhase)
                {
                    case GamePhase.Gameplay:
                        switch (e.PhaseStage)
                        {
                            case PhaseStage.EnterNormal:
                                StartRunning();
                                break;
                            case PhaseStage.LeaveNormal:
                                StopRunning();
                                break;
                        }
                        break;
                }
            });
        }
    }
}