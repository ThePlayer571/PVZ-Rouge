using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TPL.PVZR
{
    public interface ILevel
    {
        // 地图
        public Vector2Int size { get; }
        public Vector2 daveInitialPos { get; }

        public void BuildLevel();
        // == 波次生成
        // 必要数据
        public Vector2 GetZombieSpawnPosition(Level.ZombieSpawnPositionId spawnPositionId);
        public List<ZombieSpawnData> ZombieSpawnList { get; } // 本关包含的僵尸
        public Level.ZombieSpawnPositionId[] allowedSpawnPosition { get; }
        public float timeOfWave(int wave); // 第n波的时长
        public float valueOfWave(int wave); // 第n波的强度
        public int totalWaveCount { get; } // 总波数
        public int[] hugeWave { get; } // 大波的波次
        // 属性
        public int finalWave => totalWaveCount; // 最终波波次

        public Level.ZombieSpawnPositionId GetRandomSpawnPositionId(int wave)
        {
            return allowedSpawnPosition[Random.Range(0, allowedSpawnPosition.Length - 1)];
        }

        public ZombieSpawnData GetRandomZombieData(int wave)
        {
            var rand = Random.Range(0, totalWeight);
            float currentWeight = 0;
            foreach (var spawnData in ZombieSpawnList)
            {
                currentWeight+=spawnData.weight;
                if (currentWeight >= totalWeight)
                {
                    return spawnData;
                }
            }

            return ZombieSpawnData.GetDefaultData(ZombieIdentifier.NormalZombie);
        } 

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

    public class LevelDaveHouse: Level,ILevel
    {
        // 地图
        public Vector2Int size { get; } = new Vector2Int(20, 20);
        public Vector2 daveInitialPos { get; } = new Vector2(10, 10);

        public void BuildLevel()
        {
            throw new System.NotImplementedException();
        }


        // 波次生成系统
        public Vector2 GetZombieSpawnPosition(ZombieSpawnPositionId spawnPositionId)
        {
            return spawnPositionId switch
            {
                ZombieSpawnPositionId.Lawn => new Vector2(0, 0),
                ZombieSpawnPositionId.Roof => new Vector2(0, 1),
                _ => throw new System.NotImplementedException("未设置该刷怪点")
            };
        }

        public List<ZombieSpawnData> ZombieSpawnList { get; } = new()
        {
            ZombieSpawnData.GetDefaultData(ZombieIdentifier.NormalZombie)
        };

        public ZombieSpawnPositionId[] allowedSpawnPosition { get; } = { ZombieSpawnPositionId.Roof, ZombieSpawnPositionId.Lawn };

        public float valueOfWave(int wave)
        {
            return 30;
        }
        public float timeOfWave(int wave)
        {
            return 10;
        }
        public int totalWaveCount { get; } = 8;
        public int[] hugeWave { get; } = new int[] { 4 };
        public ZombieSpawnPositionId GetRandomSpawnPositionId(int wave)
        {
            if (wave <= 3)
            {
                return ZombieSpawnPositionId.Lawn;
            }
            else
            {
                return Random.value < 0.7 ? ZombieSpawnPositionId.Lawn : ZombieSpawnPositionId.Roof;
            }
        }
    }
}