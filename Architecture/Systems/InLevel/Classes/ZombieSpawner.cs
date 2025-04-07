using QFramework;
using TPL.PVZR.Gameplay.Class.Levels;
using UnityEngine;

namespace TPL.PVZR.Architecture.Systems.InLevel.Classes
{
    public class ZombieSpawner : ICanGetSystem
    {
        private IEntitySystem _EntitySystem;
        // 配置数据
        private ZombieSpawnConfig _zombieSpawnConfig;
        private float _value;
        private int _wave;
        // 
        public bool finishSpawnTask { get; private set; } = false;
        private float _spawnDeltaTime => 0.5f; // 生成间隔时间
        private float _cumulativeValue = 0; // 累计value
        private float _tryCount = 0; // 尝试次数
        private float _timer = 0;

        public void Update()
        {
            _timer += Time.deltaTime;
            if (_timer >= _spawnDeltaTime)
            {
                _timer -= _spawnDeltaTime;
                TrySpawnUntilSuccess();
            }
        }

        private void TrySpawnUntilSuccess() 
            // 逻辑：没到极限就生成一个僵尸，生成僵尸后如果超出极限，可以生成，但如果超太多了就不可以
        {
            while (_cumulativeValue < _value && _tryCount++ < 100)
            {
                Spawn(out var success);
                if (success) break;
            }

            if (_cumulativeValue >= _value || _tryCount >= 100)
            {
                finishSpawnTask = true;
            }
            
        }

        private void Spawn(out bool success)
        {
            // 获取随机zombieToSpawn
            var zombieToSpawn = _zombieSpawnConfig.GetRandomZombieData(_wave);
            // 防止过于离谱的僵尸出现
            if (_cumulativeValue + zombieToSpawn.value > _value * 1.3f)
            {
                success = false;
                return;
            }

            // 累计value
            _cumulativeValue += zombieToSpawn.value;
            // 在随机位置生成zombieToSpawn
            var spawnPosition =
                _zombieSpawnConfig.ToSpawnPosition(
                    _zombieSpawnConfig.GetRandomSpawnPositionId(_wave));
            _EntitySystem.CreateZombie(zombieToSpawn.zombieIdentifier, spawnPosition);
            //
            success = true;
            return;
        }

        public ZombieSpawner(ZombieSpawnConfig zombieSpawnConfig , float value, int wave)
        {
            _EntitySystem = this.GetSystem<IEntitySystem>();
            this._zombieSpawnConfig = zombieSpawnConfig;
            this._value = value;
            this._wave = wave;
        }

        public IArchitecture GetArchitecture()
        {
            return PVZRouge.Interface;
        }
    }
}