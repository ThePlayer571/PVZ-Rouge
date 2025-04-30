using System.Collections.Generic;
using QAssetBundle;
using QFramework;
using TPL.PVZR.Architecture.Events.GamePhase;
using TPL.PVZR.Architecture.Models;
using TPL.PVZR.Architecture.Systems.PhaseSystems;
using TPL.PVZR.Gameplay.Entities;
using TPL.PVZR.Gameplay.Entities.Zombies;
using TPL.PVZR.Gameplay.Entities.Zombies.Base;
using UnityEngine;

namespace TPL.PVZR.Architecture.Systems.InLevel
{
    
    public partial class EntitySystem : AbstractSystem, IEntitySystem
    {
        // Model|System
        private ILevelModel _LevelModel;
        private ILevelSystem _LevelSystem;

        // 初始化
        protected override void OnInit()
        {
            _LevelModel = this.GetModel<ILevelModel>();
            _LevelSystem = this.GetSystem<ILevelSystem>();
            // 初始化字典
            SetPrefabDict();
            // LevelSystem
            RegisterEvents();
        }


        # region 管理Prefab
        private void SetPrefabDict()
        {
            var _ResLoader = ResLoader.Allocate();
            // Projectile
            projectilePrefabDict = new Dictionary<ProjectileIdentifier, GameObject>()
            {
                [ProjectileIdentifier.Pea] = _ResLoader.LoadSync<GameObject>(Pea_prefab.Pea),
                [ProjectileIdentifier.IcePea] = _ResLoader.LoadSync<GameObject>(Icepea_prefab.IcePea),
                [ProjectileIdentifier.Spike] = _ResLoader.LoadSync<GameObject>(Spike_prefab.Spike),
                [ProjectileIdentifier.Cabbage] = _ResLoader.LoadSync<GameObject>(Cabbage_prefab.Cabbage),
                [ProjectileIdentifier.Watermelon] = _ResLoader.LoadSync<GameObject>(Watermelon_prefab.Watermelon),
                [ProjectileIdentifier.Kernel] = _ResLoader.LoadSync<GameObject>(Kernel_prefab.Kernel),
                [ProjectileIdentifier.Butter] = _ResLoader.LoadSync<GameObject>(Butter_prefab.Butter),
                
            };
            // Plant
            plantPrefabDict = new Dictionary<PlantIdentifier, GameObject>
            {
                [PlantIdentifier.PeaShooter] = _ResLoader.LoadSync<GameObject>(Peashooter_prefab.PeaShooter),
                [PlantIdentifier.Sunflower] = _ResLoader.LoadSync<GameObject>(Sunflower_prefab.Sunflower),
                [PlantIdentifier.Wallnut] = _ResLoader.LoadSync<GameObject>(Wallnut_prefab.Wallnut),
                [PlantIdentifier.Flowerpot] = _ResLoader.LoadSync<GameObject>(Flowerpot_prefab.Flowerpot),
                [PlantIdentifier.SnowPea] = _ResLoader.LoadSync<GameObject>(Snowpea_prefab.SnowPea),
                [PlantIdentifier.CherryBoom] = _ResLoader.LoadSync<GameObject>(Cherryboom_prefab.CherryBoom),
                [PlantIdentifier.PotatoMine] = _ResLoader.LoadSync<GameObject>(Potatomine_prefab.PotatoMine),
                [PlantIdentifier.RepeaterPea] = _ResLoader.LoadSync<GameObject>(Repeaterpea_prefab.RepeaterPea),
                [PlantIdentifier.SplitPea] = _ResLoader.LoadSync<GameObject>(Splitpea_prefab.SplitPea),
                [PlantIdentifier.Cactus] = _ResLoader.LoadSync<GameObject>(Cactus_prefab.Cactus),
                [PlantIdentifier.Caltrop] = _ResLoader.LoadSync<GameObject>(Caltrop_prefab.Caltrop),
                [PlantIdentifier.Blover] = _ResLoader.LoadSync<GameObject>(Blover_prefab.Blover),
                [PlantIdentifier.CabbagePult] = _ResLoader.LoadSync<GameObject>(Cabbagepult_prefab.CabbagePult),
                [PlantIdentifier.MelonPult] = _ResLoader.LoadSync<GameObject>(Melonpult_prefab.MelonPult),
                [PlantIdentifier.CornPult] = _ResLoader.LoadSync<GameObject>(Cornpult_prefab.CornPult),
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


        private void RegisterEvents()
        {
            this.RegisterEvent<OnEnterPhaseEvent>(e =>
            {
                if (e.changeToPhase is GamePhaseSystem.GamePhase.LevelInitialization)
                {
                    ZombieSet = new HashSet<Zombie>();
                    lastDeadZombiePosition = Vector2.zero;
                }
                else if (e.changeToPhase is GamePhaseSystem.GamePhase.LevelExiting)
                {
                    ZombieSet = null;
                }
            });
        }
    }
    
}