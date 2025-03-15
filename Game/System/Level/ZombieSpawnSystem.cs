using DG.Tweening;
using UnityEngine;
using QFramework;
using System.Collections;

namespace TPL.PVZR
{
    public interface IZombieSpawnSystem : ISystem,IInLevelSystem
    {
        public void SpawnWaveOfZombie(float value,int wave);
    }
    public class ZombieSpawnSystem: AbstractSystem,IZombieSpawnSystem
    {
        private ILevelModel _LevelModel;
        private IEntityCreateSystem _EntityCreateSystem;
        
        
        
        
        
        
        
        protected override void OnInit()
        {
            _LevelModel = this.GetModel<ILevelModel>();
            _EntityCreateSystem = this.GetSystem<IEntityCreateSystem>();
        }

        public void SpawnWaveOfZombie(float value,int wave)
        {
            float cumulativeValue = 0;
            
            ActionKit.Repeat(100)
                .Callback(() =>
                // 逻辑：没到极限就生成一个僵尸，生成僵尸后如果超出极限，可以生成，但如果超太多了就不可以
                {
                    if (cumulativeValue >= value) return;
                    //
                    // 获取随机zombieToSpawn
                    ZombieSpawnData zombieToSpawn =
                        _LevelModel.level.GetRandomZombieData(wave);
                    // 防止过于离谱的僵尸出现
                    if (cumulativeValue + zombieToSpawn.value > value * 1.3f) return;
                    // 累计value
                    cumulativeValue += zombieToSpawn.value;
                    // 在随机位置生成zombieToSpawn
                    Vector2 spawnPosition =
                        _LevelModel.level.ToSpawnPosition(_LevelModel.level.GetRandomSpawnPositionId(wave));
                    _EntityCreateSystem.CreateZombie(zombieToSpawn.zombieIdentifier, spawnPosition);
                })
                .Delay(0.5f).Start(GameManager.Instance);
        }
    }
}