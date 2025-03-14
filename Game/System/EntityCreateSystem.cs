using UnityEngine;
using QFramework;
using System.Collections.Generic;
using DG.Tweening;
using TPL.PVZR.EntitiyProjectile;
using TPL.PVZR.EntityPlant;
using TPL.PVZR.EntityZombie;

namespace TPL.PVZR
{

    public interface IEntityCreateSystem : ISystem
    {
        // Projectile
        GameObject CreatePea(ProjectileIdentifier projectileIdentifier,Vector2 position, Direction2 direction);

        // Plant
        GameObject CreatePlant(PlantIdentifier plantIdentifier, Vector2Int position,
            Direction2 direction = Direction2.Right);

        // Zombie
        GameObject CreateZombie(ZombieIdentifier zombieIdentifier, Vector2 position);

        // Others
        GameObject CreateSunBySunflower(Vector2 position);
    }

    public class EntityCreateSystem : AbstractSystem, IEntityCreateSystem
    {
        // Model|System
        private ILevelModel _LevelModel;

        

        // 初始化
        protected override void OnInit()
        {
            _LevelModel = this.GetModel<ILevelModel>();
            //
            ResLoader _ResLoader = ResLoader.Allocate();
            // Projectile
            projectilePrefabDict = new Dictionary<ProjectileIdentifier, GameObject>()
            {
                [ProjectileIdentifier.Pea] =_ResLoader.LoadSync<GameObject>("Pea"),
                [ProjectileIdentifier.IcePea] =_ResLoader.LoadSync<GameObject>("IcePea"),
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

        #region - 植物
        // 数据
        private Dictionary<PlantIdentifier, GameObject> plantPrefabDict;
        // 函数
        public GameObject CreatePlant(PlantIdentifier plantIdentifier, Vector2Int position,
            Direction2 direction = Direction2.Right)
        {
            // 创建并初始化植物
            GameObject go = GetPrefab(plantIdentifier)
                .Instantiate(new Vector3(position.x, position.y) + _LevelModel.Grid.cellSize * 0.5f,
                    Quaternion.identity);
            Plant plant = go.GetComponent<Plant>();
            plant.Initialize(direction);
            // 更改CellGrid
            Cell targetCell = _LevelModel.CellGrid[plant.gridPos2.x, plant.gridPos2.y];
            if (plantIdentifier == PlantIdentifier.Flowerpot)
            {

                targetCell.cellState = Cell.CellState.HaveFlowerpot;
            }
            else
            {
                targetCell.cellState = Cell.CellState.HavePlant;
            }
            targetCell.plant = plant;

            //
            return go;
        }
        private GameObject GetPrefab(PlantIdentifier plantIdentifier)
        {
            if (plantPrefabDict.ContainsKey(plantIdentifier))
            {
                return plantPrefabDict[plantIdentifier];
            }

            return plantPrefabDict[PlantIdentifier.PeaShooter];


        }
        #endregion

        #region - 僵尸
        // 数据
        private Dictionary<ZombieIdentifier, GameObject> zombiePrefabDict;
        // 函数
        public GameObject CreateZombie(ZombieIdentifier zombieIdentifier, Vector2 position)
        {
            GameObject go = GetPrefab(zombieIdentifier)
                .Instantiate(new Vector3(position.x, position.y), Quaternion.identity);
            go.GetComponent<Zombie>().Initialize();
            return go;
        }
        
        private GameObject GetPrefab(ZombieIdentifier zombieIdentifier)
        {
            if (zombiePrefabDict.ContainsKey(zombieIdentifier))
            {
                return zombiePrefabDict[zombieIdentifier];
            }

            return zombiePrefabDict[ZombieIdentifier.NormalZombie];


        }
        #endregion
        
        #region - 投射物
        // 数据
        private Dictionary<ProjectileIdentifier, GameObject> projectilePrefabDict;
        // 函数
        
        public GameObject CreatePea(ProjectileIdentifier projectileIdentifier,Vector2 position, Direction2 direction)
        {
            GameObject go = GetPrefab(projectileIdentifier).Instantiate(new Vector3(position.x, position.y), Quaternion.identity);
            go.GetComponent<IPea>().Initialize(direction);
            return go;
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
        
        
        private GameObject sunPrefab;
        public GameObject CreateSunBySunflower(Vector2 position)
        {
            var go = sunPrefab.Instantiate(position, Quaternion.identity);
            go.transform.DOJump(go.transform.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(0, 0.3f)),
                0.5f, 1, 0.5f);
            return go;
        }

        public GameObject CreateSunByFall(Vector2 position)
        {
            var go = sunPrefab.Instantiate(position, Quaternion.identity);
            return go;
        }



        
    }
}