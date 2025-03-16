using DG.Tweening;
using UnityEngine;
using QFramework;
using System.Collections;

namespace TPL.PVZR
{
    public interface IZombieSpawnSystem : ISystem, IInLevelSystem
    { 
        void SpawnWaveOfZombie(float value, int wave);
        int OperatingCoroutine { get; }
    }

    public class ZombieSpawnSystem : AbstractSystem, IZombieSpawnSystem
    {
        private ILevelModel _LevelModel;
        private IEntitySystem _EntitySystem;

        public int OperatingCoroutine { get; private set; } = 0;





        protected override void OnInit()
        {
            _LevelModel = this.GetModel<ILevelModel>();
            _EntitySystem = this.GetSystem<IEntitySystem>();
        }

        public void SpawnWaveOfZombie(float value, int wave)
        {
            GameManager.Instance.StartCoroutine(Func_1());
            IEnumerator Func_1()
                // 逻辑：没到极限就生成一个僵尸，生成僵尸后如果超出极限，可以生成，但如果超太多了就不可以
            {
                OperatingCoroutine++;
                //
                float cumulativeValue = 0; // 累计value
                float tryCount = 0; // 尝试次数
                while (cumulativeValue < value && tryCount++ < 100)
                {
                    // = 生成僵尸
                    // 获取随机zombieToSpawn
                    ZombieSpawnData zombieToSpawn =
                        _LevelModel.level.GetRandomZombieData(wave);
                    // 防止过于离谱的僵尸出现
                    if (cumulativeValue + zombieToSpawn.value > value * 1.3f) continue;
                    // 累计value
                    cumulativeValue += zombieToSpawn.value;
                    // 在随机位置生成zombieToSpawn
                    Vector2 spawnPosition =
                        _LevelModel.level.ToSpawnPosition(_LevelModel.level.GetRandomSpawnPositionId(wave));
                    _EntitySystem.CreateZombie(zombieToSpawn.zombieIdentifier, spawnPosition);

                    //
                    yield return new WaitForSeconds(Random.Range(0.5f,0.8f));
                }
                // 结束
                OperatingCoroutine--;
            }
        }
    }
}