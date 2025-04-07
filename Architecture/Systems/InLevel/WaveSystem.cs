using QFramework;
using TPL.PVZR.Architecture.Events;
using TPL.PVZR.Architecture.Events.GamePhase;
using TPL.PVZR.Architecture.Managers;
using TPL.PVZR.Architecture.Models;
using TPL.PVZR.Architecture.Systems.PhaseSystems;
using UnityEngine;

namespace TPL.PVZR.Architecture.Systems.InLevel
{
    /// <summary>
    /// 管理记录波次/处理波次行为
    /// </summary>
    public interface IWaveSystem : ISystem
    {
        int currentWave { get; } // 已经生成了第n波怪时，currentWave = n
    }

    public class WaveSystem : AbstractSystem, IWaveSystem
    {
        // 
        private ILevelModel _LevelModel;

        private IZombieSpawnSystem _ZombieSpawnSystem;

        // 运行时变量
        public int currentWave { get; private set; } = 0;
        private float waveTimer = 0;




        protected override void OnInit()
        {
            _LevelModel = this.GetModel<ILevelModel>();
            _ZombieSpawnSystem = this.GetSystem<IZombieSpawnSystem>();
            // LevelSystem
            RegisterEvents();
        }

        #region 一层形象

        
        private void StartWaveSystem()
        {
            GameManager.ExecuteOnUpdate(Update);
        }
        private void EndWaveSystem()
        {
            GameManager.StopOnUpdate(Update);
        }

        #endregion
        
        private void RegisterEvents()
        {
            this.RegisterEvent<OnEnterPhaseEvent>(e =>
            {
                if (e.changeToPhase is GamePhaseSystem.GamePhase.LevelInitialization)
                {
                    currentWave = 0;
                    waveTimer = 0;
                }else if (e.changeToPhase is GamePhaseSystem.GamePhase.LevelExiting)
                {
                    currentWave = 0;
                    waveTimer = 0;
                }else if (e.changeToPhase is GamePhaseSystem.GamePhase.Gameplay)
                {
                    StartWaveSystem();
                }else if (e.changeToPhase is GamePhaseSystem.GamePhase.LevelExiting)
                {
                    EndWaveSystem();
                    
                }
            });
        }
        private void Update()
        {
            // 在刷新第1波怪之前的时间叫做第0波
            waveTimer += Time.deltaTime;
            if (waveTimer >= _LevelModel.WaveConfig.timeOfWave(currentWave))
            {
                StartNextWave();
            }

            if (currentWave == _LevelModel.WaveConfig.totalWaveCount)
            {
                GameManager.StopOnUpdate(Update);
            }
        }

        private void StartNextWave()
        {
            waveTimer = 0;
            currentWave++;
            //
            this.SendEvent<WaveStartEvent>(new WaveStartEvent { wave = currentWave });
        }

    }
}