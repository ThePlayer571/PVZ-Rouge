using QFramework;
using UnityEngine;

namespace TPL.PVZR
{
    public interface IWaveSystem:ISystem,IInLevelSystem
    {
        
    }
    public class WaveSystem:AbstractSystem,IWaveSystem
    {
        // 
        private ILevelModel _LevelModel;
        private IZombieSpawnSystem _ZombieSpawnSystem;
        // 运行时变量
        public int currentWave { get;private set; }
        private float waveTimer = 0;


        public void OnGameplay()
        {
            StartWave();
        }

        private void StartWave()
        {
            "开始波次".LogInfo();
            GameManager.ExecuteOnUpdate(Update);
        }

        private void Update()
        {
            waveTimer += Time.deltaTime;
            if (waveTimer >= _LevelModel.level.timeOfWave(currentWave))
            {
                waveTimer = 0;
                _ZombieSpawnSystem.SpawnWaveOfZombie(_LevelModel.level.valueOfWave(currentWave),currentWave);
                currentWave++;
            }
        }


        protected override void OnInit()
        {
            _LevelModel = this.GetModel<ILevelModel>();
            _ZombieSpawnSystem = this.GetSystem<IZombieSpawnSystem>();
        }
        
    }
}