using QFramework;
using TPL.PVZR.CommandEvents.Level_Gameplay.Waves;
using TPL.PVZR.CommandEvents.Phase;
using TPL.PVZR.Models;
using TPL.PVZR.Tools;
using TPL.PVZR.ViewControllers.Managers;
using UnityEngine;

namespace TPL.PVZR.Systems
{
    public interface IWaveSystem : ISystem
    {
        BindableProperty<int> CurrentWave { get; }
    }

    public class WaveSystem : AbstractSystem, IWaveSystem
    {
        private ILevelModel _LevelModel;
        public BindableProperty<int> CurrentWave { get; private set; }

        private Timer WaveTimer;

        private void Update()
        {
            WaveTimer.Update(Time.deltaTime);

            if (WaveTimer.Ready)
            {
                CurrentWave.Value++;
                WaveTimer.SetRemaining(_LevelModel.LevelData.DurationOfWave(CurrentWave.Value));
                this.SendEvent<OnWaveStart>(new OnWaveStart {Wave = CurrentWave.Value});
                $"发送事件 : Wave{CurrentWave.Value}".LogInfo();
            }
        }

        private void Start()
        {
            CurrentWave.Value = 0;
            WaveTimer.SetRemaining(_LevelModel.LevelData.DurationOfWave(0));
        }


        protected override void OnInit()
        {
            _LevelModel = this.GetModel<ILevelModel>();

            CurrentWave = new BindableProperty<int>(0);
            WaveTimer = new Timer(0);

            this.RegisterEvent<OnPhaseChangeEvent>(e =>
            {
                switch (e.GamePhase)
                {
                    case GamePhase.Gameplay:
                        switch (e.PhaseStage)
                        {
                            case PhaseStage.EnterNormal:
                                GameManager.ExecuteOnUpdate(Update);
                                Start();
                                break;
                            case PhaseStage.LeaveNormal:
                                GameManager.StopOnUpdate(Update);
                                break;
                        }

                        break;
                }
            });
        }
    }
}