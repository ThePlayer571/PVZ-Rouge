using System.Collections.Generic;
using DG.Tweening;
using QFramework;
using TPL.PVZR.Architecture.Events;
using TPL.PVZR.Architecture.Models;
using TPL.PVZR.Core;
using TPL.PVZR.Gameplay.Class;
using TPL.PVZR.Gameplay.Entities;
using TPL.PVZR.Gameplay.Entities.Plants.Base;
using TPL.PVZR.Gameplay.Entities.Projectiles;
using TPL.PVZR.Gameplay.Entities.Zombies;
using UnityEngine;

namespace TPL.PVZR.Architecture.Systems.InLevel
{
    public interface IEntitySystem : ISystem
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
    public partial class EntitySystem
    {
        #region 实体记录

        
        public HashSet<Zombie> ZombieSet { get; private set; } = new(); // 存储所有场上的僵尸

        public Vector2 lastDeadZombiePosition { get; private set; } 

        #endregion
        
        # region 创建实体
        public GameObject CreatePlant(PlantIdentifier plantIdentifier, Vector2Int position,
            Direction2 direction = Direction2.Right)
        {
            // 创建并初始化植物
            GameObject go = GetPrefab(plantIdentifier)
                .Instantiate(new Vector3(position.x, position.y) + ReferenceModel.Get.Grid.cellSize * 0.5f,
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

        public GameObject CreateZombie(ZombieIdentifier zombieIdentifier, Vector2 position)
        {
            // 创建
            GameObject go = GetPrefab(zombieIdentifier)
                .Instantiate(new Vector3(position.x, position.y), Quaternion.identity);
            go.GetComponent<Zombie>().Initialize();
            // 记录
            ZombieSet.Add(go.GetComponent<Zombie>());
            return go;
        }

        public GameObject CreatePea(ProjectileIdentifier projectileIdentifier, Vector2 position, Direction2 direction)
        {
            GameObject go = GetPrefab(projectileIdentifier)
                .Instantiate(new Vector3(position.x, position.y), Quaternion.identity);
            go.GetComponent<IPea>().Initialize(direction);
            return go;
        }

        public GameObject CreateSunByFall(Vector2 position)
        {
            var go = sunPrefab.Instantiate(position, Quaternion.identity);
            return go;
        }
        public GameObject CreateSunBySunflower(Vector2 position)
        {
            var go = sunPrefab.Instantiate(position, Quaternion.identity);
            go.transform.DOJump(go.transform.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(0, 0.3f)),
                0.5f, 1, 0.5f);
            return go;
        }
        # endregion

        #region 删除实体

        
        public void DestroyZombie(Zombie zombie)
        {
            ZombieSet.Remove(zombie);
            lastDeadZombiePosition= zombie.transform.position;
            // 触发事件
            this.SendEvent<OnZombieDestroyedEvent>();
        }

        #endregion
    }

}