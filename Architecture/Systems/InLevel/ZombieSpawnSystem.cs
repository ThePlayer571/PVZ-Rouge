using System.Collections.Generic;
using System.Linq;
using QFramework;
using TPL.PVZR.Architecture.Events;
using TPL.PVZR.Architecture.Events.GamePhase;
using TPL.PVZR.Architecture.Managers;
using TPL.PVZR.Architecture.Models;
using TPL.PVZR.Architecture.Systems.InLevel.Classes;
using TPL.PVZR.Architecture.Systems.PhaseSystems;

namespace TPL.PVZR.Architecture.Systems.InLevel
{
    /// <summary>
    /// 记录僵尸生成器/处理僵尸生成事件
    /// </summary>
    public interface IZombieSpawnSystem : ISystem
    { 
        /// <summary>
        /// 召唤一波僵尸
        /// </summary>
        /// <param name="value">僵尸总强度</param>
        /// <param name="wave">当前波数</param>
        void SpawnWaveOfZombie(float value, int wave);
        HashSet<ZombieSpawner> activeZombieSpawners { get; }
    }


    public class ZombieSpawnSystem : AbstractSystem, IZombieSpawnSystem
    {
        private ILevelModel _LevelModel;
        private IEntitySystem _EntitySystem;
        
        //
        
        public void SpawnWaveOfZombie(float value, int wave)
        {
            activeZombieSpawners.Add(new ZombieSpawner(_LevelModel.ZombieSpawnConfig, value, wave));
        }
        public HashSet<ZombieSpawner> activeZombieSpawners { get; private set; }

        protected override void OnInit()
        {
            _LevelModel = this.GetModel<ILevelModel>();
            _EntitySystem = this.GetSystem<IEntitySystem>();
            //
            RegisterEvents();
        }

        private void RegisterEvents()
        {
            // PhaseEvents
            this.RegisterEvent<OnEnterPhaseEvent>(e =>
            {
                if (e.changeToPhase is GamePhaseSystem.GamePhase.LevelInitialization)
                {
                    activeZombieSpawners = new HashSet<ZombieSpawner>();
                }else if (e.changeToPhase is GamePhaseSystem.GamePhase.Gameplay)
                {
                    GameManager.ExecuteOnUpdate(Update);
                }else if (e.changeToPhase is GamePhaseSystem.GamePhase.LevelExiting)
                {
                    GameManager.StopOnUpdate(Update);
                    activeZombieSpawners = null;
                }
            });
            // 波次事件
            this.RegisterEvent<WaveStartEvent>(e =>
            {
                SpawnWaveOfZombie(_LevelModel.WaveConfig.valueOfWave(e.wave), e.wave);
            });
        }
        
        private void Update()
        {
            foreach (var zombieSpawner in activeZombieSpawners.ToArray())
            {
                if (zombieSpawner.finishSpawnTask)
                {
                    activeZombieSpawners.Remove(zombieSpawner);
                }
                else
                {
                    zombieSpawner.Update();
                }
            }
        }

    }
}