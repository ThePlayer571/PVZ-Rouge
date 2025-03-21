using System.Collections.Generic;
using System.Linq;
using QFramework;
using UnityEngine;

namespace TPL.PVZR
{

    public abstract class MapConfig
    {
        public abstract Vector2Int size { get; protected set; }
        public abstract Vector2 daveInitialPos { get; protected set; }
        public abstract GameObject GetLevelSceneSet();
    }
    public abstract class WaveConfig
    {
        public abstract int totalWaveCount { get; protected set; } // 总波数
        public abstract int[] hugeWaves { get; protected set; } // 大波的波次
        public int finalWave => totalWaveCount;// 最终波波次
        public abstract float timeOfWave(int wave); // 第n波的时长
        public abstract float valueOfWave(int wave); // 第n波的强度
        public virtual List<int> flaggedWaves // 需要被旗子标记的波次
        {
            
            get
            {
                var result = new List<int>(hugeWaves) { finalWave };
                return result;
            }
        }
    }
    public abstract class ZombieSpawnConfig
    {
        
        protected abstract List<ZombieSpawnData> ZombieSpawnList { get; set; } // 僵尸生成数据

        public float totalWeight => ZombieSpawnList.Sum(eachData => eachData.weight); // 僵尸总权重
        public abstract Level.ZombieSpawnPositionId[] allowedSpawnPosition { get; protected set;} // 僵尸生产点
        public abstract Vector2 ToSpawnPosition(Level.ZombieSpawnPositionId spawnPositionId); // 转换僵尸生成坐标
        
        // 僵尸列表
        public virtual ZombieSpawnData GetRandomZombieData(int wave) // 按权重 获取随机的僵尸
        {
            var randomWeight = Random.Range(0, totalWeight);
            float currentWeight = 0;
            foreach (var spawnData in ZombieSpawnList)
            {
                currentWeight+=spawnData.weight;
                if (currentWeight >= randomWeight)
                {
                    return spawnData;
                }
            }
            return ZombieSpawnData.GetDefaultData(ZombieIdentifier.NormalZombie);
        }
        // 僵尸位置

        public abstract Level.ZombieSpawnPositionId GetRandomSpawnPositionId(int wave); // 获取随机的僵尸生成点


    }

    public abstract class LootConfig
    {
        public abstract List<LootData> LootDataList { get; protected set; } // 战利品列表
        public abstract float value { get; } // 战利品总值
    }
}