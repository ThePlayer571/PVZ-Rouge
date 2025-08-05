using QFramework;
using TPL.PVZR.CommandEvents.Level_Gameplay.Waves;
using TPL.PVZR.Models;
using TPL.PVZR.Services;
using TPL.PVZR.Tools;
using TPL.PVZR.Tools.SoyoFramework;
using TPL.PVZR.ViewControllers.Managers;
using UnityEngine;

namespace TPL.PVZR.Systems.Level_Event
{
    public interface IWaveSystem : IAutoUpdateSystem
    {
    }

    public class WaveSystem : AbstractSystem, IWaveSystem
    {
        private void Update()
        {
            WaveTimer.Update(Time.deltaTime);

            if (WaveTimer.Ready)
            {
                _LevelModel.CurrentWave.Value++;
                WaveTimer.SetRemaining(_LevelModel.LevelData.DurationOfWave(_LevelModel.CurrentWave.Value));
                this.SendEvent<OnWaveStart>(new OnWaveStart { Wave = _LevelModel.CurrentWave.Value });
                if (_LevelModel.CurrentWave.Value == _LevelModel.LevelData.TotalWaveCount)
                {
                    this.SendEvent<OnFinalWaveStart>();
                }
            }
        }

        private void StartRunning()
        {
            _LevelModel.CurrentWave.Value = 0;
            WaveTimer.SetRemaining(_LevelModel.LevelData.DurationOfWave(0));

            GameManager.ExecuteOnUpdate(Update);
        }

        private void StopRunning()
        {
            GameManager.StopOnUpdate(Update);
        }

        private ILevelModel _LevelModel;

        private Timer WaveTimer;

        protected override void OnInit()
        {
            _LevelModel = this.GetModel<ILevelModel>();

            WaveTimer = new Timer(0);

            var phaseService = this.GetService<IPhaseService>();
            phaseService.RegisterCallBack((GamePhase.Gameplay, PhaseStage.EnterNormal), e => { StartRunning(); });
            phaseService.RegisterCallBack((GamePhase.Gameplay, PhaseStage.LeaveNormal), e => { StopRunning(); });
        }
    }
}