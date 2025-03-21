using QFramework;
using UnityEngine;
using DG.Tweening;

namespace TPL.PVZR
{
    public interface IWaveSystem : ISystem, IInLevelSystem
    {
        int currentWave { get; } // 已经生成了第n波怪时，currentWave = n
}
    public class WaveSystem:AbstractSystem,IWaveSystem
    {
        // 
        private ILevelModel _LevelModel;
        private IZombieSpawnSystem _ZombieSpawnSystem;
        // 运行时变量
        public int currentWave { get; private set; } = 0;
        private float waveTimer = 0;


        public void OnExiting()
        {
            currentWave = 0;
            waveTimer = 0;
        }

        public void OnBuildingLevel()
        {
            currentWave = 0;
            waveTimer = 0;
        }
        
        
        public void OnGameplay()
        {
            StartWaveSystem();
        }

        private void StartWaveSystem()
        {
            GameManager.ExecuteOnUpdate(Update);
        }

        private void EndWaveSystem()
        {
            GameManager.StopOnUpdate(Update);
        }

        private void Update()
        {
            // 在刷新第1波怪之前的时间叫做第0波
            waveTimer += Time.deltaTime;
            if (waveTimer >= _LevelModel.WaveConfig.timeOfWave(currentWave))
            {
                StartNextWave();}

            if (currentWave == _LevelModel.WaveConfig.totalWaveCount)
            {
                GameManager.StopOnUpdate(Update);
            }
        }

        private void StartNextWave()
        {
            waveTimer = 0;
            currentWave++;
            _ZombieSpawnSystem.SpawnWaveOfZombie(_LevelModel.WaveConfig.valueOfWave(currentWave),currentWave);
            this.SendEvent<WaveStartEvent>(new WaveStartEvent { wave = currentWave });

        }


        protected override void OnInit()
        {
            _LevelModel = this.GetModel<ILevelModel>();
            _ZombieSpawnSystem = this.GetSystem<IZombieSpawnSystem>();
        }
        
    }
}