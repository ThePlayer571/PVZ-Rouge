using System.Collections.Generic;
using System.Linq;
using QFramework;
using TPL.PVZR.Core;
using TPL.PVZR.Core.Random;
using TPL.PVZR.Gameplay.Entities;
using UnityEngine;

namespace TPL.PVZR.Gameplay.Class.Levels.Levels.DaveLawn
{

    public class DaveLawn: Level
    {
        public override MapConfig MapConfig { get; } = new MapConfigDaveLawn();
        public override WaveConfig WaveConfig { get; } = new WaveConfigDaveLawn();
        public override  ZombieSpawnConfig ZombieSpawnConfig { get; } = new ZombieSpawnConfigDaveLawn();
        public override LootConfig LootConfig { get; } = new LootConfigDaveLawn();
    }
    
    public class MapConfigDaveLawn : MapConfig
    {
        public override Vector2Int size { get; protected set; } = new Vector2Int(47, 28);
        public override Vector2 daveInitialPos { get; protected set; } = new Vector2(10, 10);
        public override GameObject GetLevelSceneSet()
        {
            return ResLoader.Allocate().LoadSync<GameObject>("LevelSceneSet_DaveHouse");
        }
    }

    public class WaveConfigDaveLawn : WaveConfig
    {
        public override int totalWaveCount { get; protected set; } = 8;
        public override int[] hugeWaves { get; protected set; } = new int[] { 4 };
        public override float timeOfWave(int wave)
        {
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

    public class ZombieSpawnConfigDaveLawn : ZombieSpawnConfig
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
                return RandomHelper.Default.Value < 0.7 ? Level.ZombieSpawnPositionId.Lawn : Level.ZombieSpawnPositionId.Roof;
            }
        }
    }
    
    public class LootConfigDaveLawn : LootConfig
    {
        public override List<Loot> LootDataList { get; protected set; } = new List<Loot>
        {
            Loot.GetDefaultData(PlantIdentifier.PeaShooter), Loot.GetDefaultData(PlantIdentifier.Flowerpot),
            Loot.GetDefaultData(PlantIdentifier.Sunflower), Loot.GetDefaultData(PlantIdentifier.CherryBoom),
            Loot.GetDefaultData(PlantIdentifier.SnowPea),Loot.GetDefaultData(PlantIdentifier.PotatoMine),
        };

        public override float value => RandomHelper.Game.Range(300f, 400f);
    }
}