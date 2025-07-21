using QFramework;
using TPL.PVZR.CommandEvents.Level_Gameplay.Waves;
using TPL.PVZR.CommandEvents.Phase;
using TPL.PVZR.Models;
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
                _LevelModel.   CurrentWave.Value++;
                WaveTimer.SetRemaining(_LevelModel.LevelData.DurationOfWave(_LevelModel.CurrentWave.Value));
                this.SendEvent<OnWaveStart>(new OnWaveStart { Wave = _LevelModel.CurrentWave.Value });
            }
        }

        private void StartRunning()
        {
            _LevelModel. CurrentWave.Value = 0;
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