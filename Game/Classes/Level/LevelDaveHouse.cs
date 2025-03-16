using System;
using System.Collections.Generic;
using System.Linq;
using QFramework;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TPL.PVZR
{
    public class LevelDaveHouse: Level,ILevel
    {
        // 地图
        public Vector2Int size { get; } = new Vector2Int(45, 26);
        public Vector2 daveInitialPos { get; } = new Vector2(10, 10);


        // 波次生成系统

        public Vector2 ToSpawnPosition(ZombieSpawnPositionId spawnPositionId)
        {
            return spawnPositionId switch
            {
                ZombieSpawnPositionId.Lawn => new Vector2(43.5f, 3.8f),
                ZombieSpawnPositionId.Roof => new Vector2(2.5f, 13.6f), // TODO: AI做好了要删的
                _ => throw new System.NotImplementedException("未设置该刷怪点")
            };
        }

        public List<ZombieSpawnData> ZombieSpawnList { get; } = new()
        {
            ZombieSpawnData.GetDefaultData(ZombieIdentifier.NormalZombie),
            ZombieSpawnData.GetDefaultData(ZombieIdentifier.ConeheadZombie)
        };

        public ZombieSpawnPositionId[] allowedSpawnPosition { get; } = { ZombieSpawnPositionId.Roof, ZombieSpawnPositionId.Lawn };

        public float valueOfWave(int wave)
        {
            if (wave == totalWaveCount) // 最后一波
            {
                return (wave - 1) * 10f + 10;
            }
            else if (hugeWaves.Contains(wave)) // 大波
            {
                return (wave - 1) * 7f + 10;
            }
            else // 普通波
            {
                return (wave - 1) * 7f + 10;
            }
        }
        public float timeOfWave(int wave)
        {
            return 10f;
        }
        public int totalWaveCount { get; } = 8;
        public int[] hugeWaves { get; } = new int[] { 4 };
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

        public GameObject GetLevelSceneSet()
        {
            return ResLoader.Allocate().LoadSync<GameObject>("LevelSceneSet_DaveHouse");
        }
    }
}