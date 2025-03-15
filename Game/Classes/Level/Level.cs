using System;
using System.Collections.Generic;
using System.Linq;
using QFramework;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TPL.PVZR
{
    public interface ILevel
    {
        // 地图
        public Vector2Int size { get; }
        public Vector2 daveInitialPos { get; }
        public GameObject GetLevelSceneSet();
        // == 波次生成
        // 僵尸列表
        public List<ZombieSpawnData> ZombieSpawnList { get; } // 本关包含的僵尸
        public ZombieSpawnData GetRandomZombieData(int wave)
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
        public Level.ZombieSpawnPositionId[] allowedSpawnPosition { get; } // 本关生成僵尸的可能点
        public Vector2 ToSpawnPosition(Level.ZombieSpawnPositionId spawnPositionId);
        public Level.ZombieSpawnPositionId GetRandomSpawnPositionId(int wave)
        {
            return allowedSpawnPosition[Random.Range(0, allowedSpawnPosition.Length - 1)];
        }
        // 数据
        public float timeOfWave(int wave); // 第n波的时长
        public float valueOfWave(int wave); // 第n波的强度
        public int totalWaveCount { get; } // 总波数
        public int[] hugeWave { get; } // 大波的波次
        public int finalWave => totalWaveCount; // 最终波波次
        public float totalWeight
        {
            get
            {
                return ZombieSpawnList.Sum(eachData => eachData.weight);
            }
        } 

    }
    public abstract class Level
    {
        public enum ZombieSpawnPositionId
        {
            Lawn, Roof
        }
    }

    
}