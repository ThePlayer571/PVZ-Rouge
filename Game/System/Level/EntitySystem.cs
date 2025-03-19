using UnityEngine;
using QFramework;
using System.Collections.Generic;
using DG.Tweening;
using TPL.PVZR.EntitiyProjectile;
using TPL.PVZR.EntityPlant;
using TPL.PVZR.EntityZombie;

namespace TPL.PVZR
{

    public interface IEntitySystem : ISystem, IInLevelSystem
    {
        #region 实体记录

        public HashSet<Zombie> ZombieSet { get; }
        public Vector2 lastDeadZombiePosition { get; }

        #endregion

        #region 创建实体

        // Projectile
        GameObject CreatePea(ProjectileIdentifier projectileIdentifier, Vector2 position, Direction2 direction);

        // Plant
        GameObject CreatePlant(PlantIdentifier plantIdentifier, Vector2Int position,
            Direction2 direction = Direction2.Right);

        // Zombie
        GameObject CreateZombie(ZombieIdentifier zombieIdentifier, Vector2 position);

        // Others
        GameObject CreateSunBySunflower(Vector2 position);

        #endregion

        #region 删除实体

        void DestroyZombie(Zombie zombie);

        #endregion
    }

    public partial class EntitySystem : AbstractSystem, IEntitySystem
    {
        // Model|System
        private ILevelModel _LevelModel;
        private ILevelSystem _LevelSystem;

        // 数据
        public HashSet<Zombie> ZombieSet { get; private set; } = new();


        // 初始化
        protected override void OnInit()
        {
            _LevelModel = this.GetModel<ILevelModel>();
            _LevelSystem = this.GetSystem<ILevelSystem>();
            // 初始化字典
            SetPrefabDict();
        }
# region 管理Prefab
        private void SetPrefabDict()
        {
            var _ResLoader = ResLoader.Allocate();
            // Projectile
            projectilePrefabDict = new Dictionary<ProjectileIdentifier, GameObject>()
            {
                [ProjectileIdentifier.Pea] = _ResLoader.LoadSync<GameObject>("Pea"),
                [ProjectileIdentifier.IcePea] = _ResLoader.LoadSync<GameObject>("IcePea"),
            };
            // Plant
            plantPrefabDict = new Dictionary<PlantIdentifier, GameObject>
            {
                [PlantIdentifier.PeaShooter] = _ResLoader.LoadSync<GameObject>("PeaShooter"),
                [PlantIdentifier.Sunflower] = _ResLoader.LoadSync<GameObject>("Sunflower"),
                [PlantIdentifier.Wallnut] = _ResLoader.LoadSync<GameObject>("Wallnut"),
                [PlantIdentifier.Flowerpot] = _ResLoader.LoadSync<GameObject>("Flowerpot"),
                [PlantIdentifier.SnowPea] = _ResLoader.LoadSync<GameObject>("SnowPea"),
                [PlantIdentifier.CherryBoom] = _ResLoader.LoadSync<GameObject>("CherryBoom"),
                [PlantIdentifier.PotatoMine] = _ResLoader.LoadSync<GameObject>("PotatoMine"),

            };
            // Zombie
            zombiePrefabDict = new Dictionary<ZombieIdentifier, GameObject>
            {
                [ZombieIdentifier.NormalZombie] = _ResLoader.LoadSync<GameObject>("NormalZombie"),
                [ZombieIdentifier.ConeheadZombie] = _ResLoader.LoadSync<GameObject>("ConeheadZombie"),
                [ZombieIdentifier.ScreenDoorZombie] = _ResLoader.LoadSync<GameObject>("ScreenDoorZombie"),
                [ZombieIdentifier.BucketZombie] = _ResLoader.LoadSync<GameObject>("BucketZombie"),
            };
            // Others
            sunPrefab = _ResLoader.LoadSync<GameObject>("Sun");
        }

        // 数据
        private Dictionary<PlantIdentifier, GameObject> plantPrefabDict;
        private Dictionary<ZombieIdentifier, GameObject> zombiePrefabDict;
        private Dictionary<ProjectileIdentifier, GameObject> projectilePrefabDict;
        private GameObject sunPrefab;
        // 函数
        private GameObject GetPrefab(PlantIdentifier plantIdentifier)
        {
            if (plantPrefabDict.ContainsKey(plantIdentifier))
            {
                return plantPrefabDict[plantIdentifier];
            }

            return plantPrefabDict[PlantIdentifier.PeaShooter];
        }
        private GameObject GetPrefab(ZombieIdentifier zombieIdentifier)
        {
            if (zombiePrefabDict.ContainsKey(zombieIdentifier))
            {
                return zombiePrefabDict[zombieIdentifier];
            }

            return zombiePrefabDict[ZombieIdentifier.NormalZombie];
        }
        private GameObject GetPrefab(ProjectileIdentifier projectileIdentifier)
        {
            if (projectilePrefabDict.ContainsKey(projectileIdentifier))
            {
                return projectilePrefabDict[projectileIdentifier];
            }

            return projectilePrefabDict[ProjectileIdentifier.Pea];
        }

        #endregion
        public void DestroyZombie(Zombie zombie)
        {
            ZombieSet.Remove(zombie);
            lastDeadZombiePosition= zombie.transform.position;
            // 触发一些事件
            _LevelSystem.TryEndLevel();
        }

        public Vector2 lastDeadZombiePosition { get; private set; } 

    }
    
}