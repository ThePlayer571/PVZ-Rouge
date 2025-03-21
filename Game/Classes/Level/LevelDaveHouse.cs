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
        public MapConfig MapConfig { get; } = new MapConfigLevelDaveHouse();
        public WaveConfig WaveConfig { get; } = new WaveConfigLevelDaveHouse();
        public ZombieSpawnConfig ZombieSpawnConfig { get; } = new ZombieSpawnConfigLevelDaveHouse();
        public LootConfig LootConfig { get; } = new LootConfigLevelDaveHouse();
    }
    
    public class MapConfigLevelDaveHouse : MapConfig
    {
        public override Vector2Int size { get; protected set; } = new Vector2Int(45, 26);
        public override Vector2 daveInitialPos { get; protected set; } = new Vector2(10, 10);
        public override GameObject GetLevelSceneSet()
        {
            return ResLoader.Allocate().LoadSync<GameObject>("LevelSceneSet_DaveHouse");
        }
    }

    public class WaveConfigLevelDaveHouse : WaveConfig
    {
        public override int totalWaveCount { get; protected set; } = 8;
        public override int[] hugeWaves { get; protected set; } = new int[] { 4 };
        public override float timeOfWave(int wave)
        {
            if (wave == 0)
            {
                return 1;
            }
            return 1;
        }

        public override float valueOfWave(int wave)
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
                return (wave - 1) * 5f + 10;
            }
        }
    }

    public class ZombieSpawnConfigLevelDaveHouse : ZombieSpawnConfig
    {
        protected override List<ZombieSpawnData> ZombieSpawnList { get; set; } = new()
        {
            ZombieSpawnData.GetDefaultData(ZombieIdentifier.NormalZombie),
            ZombieSpawnData.GetDefaultData(ZombieIdentifier.ConeheadZombie)
        };

        public override Level.ZombieSpawnPositionId[] allowedSpawnPosition { get; protected set; } =
            { Level.ZombieSpawnPositionId.Roof, Level.ZombieSpawnPositionId.Lawn };



        public override Vector2 ToSpawnPosition(Level.ZombieSpawnPositionId spawnPositionId)
        {
            return spawnPositionId switch
            {
                Level.ZombieSpawnPositionId.Lawn => new Vector2(43.5f, 3.8f),
                Level.ZombieSpawnPositionId.Roof => new Vector2(2.5f, 13.6f), // TODO: AI做好了要删的
                _ => throw new System.NotImplementedException("未设置该刷怪点")
            };
        }
        
        public override Level.ZombieSpawnPositionId GetRandomSpawnPositionId(int wave)
        {
            if (wave <= 3)
            {
                return Level.ZombieSpawnPositionId.Lawn;
            }
            else
            {
                return Random.value < 0.7 ? Level.ZombieSpawnPositionId.Lawn : Level.ZombieSpawnPositionId.Roof;
            }
        }
    }
    
    public class LootConfigLevelDaveHouse : LootConfig
    {
        public override List<LootData> LootDataList { get; protected set; } = new List<LootData>
        {
            LootData.GetDefaultData(PlantIdentifier.PeaShooter), LootData.GetDefaultData(PlantIdentifier.Flowerpot),
            LootData.GetDefaultData(PlantIdentifier.Sunflower), LootData.GetDefaultData(PlantIdentifier.CherryBoom),
            LootData.GetDefaultData(PlantIdentifier.SnowPea),LootData.GetDefaultData(PlantIdentifier.PotatoMine),
        };

        public override float value => Random.Range(300f, 400f);
    }
}