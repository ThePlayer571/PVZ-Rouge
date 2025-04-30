using System;
using System.Collections.Generic;
using DG.Tweening;
using QFramework;
using TPL.PVZR.Architecture.Events;
using TPL.PVZR.Architecture.Managers;
using TPL.PVZR.Architecture.Models;
using TPL.PVZR.Core;
using TPL.PVZR.Gameplay.Class;
using TPL.PVZR.Gameplay.Entities;
using TPL.PVZR.Gameplay.Entities.Plants.Base;
using TPL.PVZR.Gameplay.Entities.Projectiles;
using TPL.PVZR.Gameplay.Entities.Zombies;
using TPL.PVZR.Gameplay.Entities.Zombies.Base;
using UnityEngine;
using Random = UnityEngine.Random;

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
        GameObject CreateIPea(ProjectileIdentifier projectileIdentifier, Vector2 position, Direction2 direction);
        GameObject CreateICabbage(ProjectileIdentifier projectileIdentifier, Vector2 position, Direction2 direction);

        // Plant
        GameObject CreatePlant(PlantIdentifier plantIdentifier, Vector2Int position,
            Direction2 direction = Direction2.Right);

        // Zombie
        GameObject CreateZombie(ZombieIdentifier zombieIdentifier, Vector2 position);

        // Others
        GameObject CreateSunBySunflower(Vector2 position);

        #endregion

        #region 删除实体

        void RemoveZombie(Zombie zombie);

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
                targetCell.SetState(Cell.CellState.HaveFlowerpot);
            }
            else
            {
                targetCell.SetState(Cell.CellState.HavePlant);
            }

            targetCell.SetPlant(plant);

            //
            return go;
        }

        public GameObject CreateZombie(ZombieIdentifier zombieIdentifier, Vector2 position)
        {
            // 创建
            GameObject go = GetPrefab(zombieIdentifier)
                .Instantiate(new Vector3(position.x, position.y), Quaternion.identity);
            // 记录
            ZombieSet.Add(go.GetComponent<Zombie>());
            return go;
        }

        public GameObject CreateIPea(ProjectileIdentifier projectileIdentifier, Vector2 position, Direction2 direction)
        {
            var go = GetPrefab(projectileIdentifier)
                .Instantiate(new Vector3(position.x, position.y), Quaternion.identity);
            var pea = go.GetComponent<IPea>();
            if (pea is null) throw new ArgumentException($"该投射物不是IPea: {projectileIdentifier}");
            pea.Initialize(direction);
            return go;
        }

        public GameObject CreateICabbage(ProjectileIdentifier projectileIdentifier, Vector2 position,
            Direction2 direction)
        {
            var go = GetPrefab(projectileIdentifier)
                .Instantiate(new Vector3(position.x, position.y), Quaternion.identity);
            var cabbage = go.GetComponent<ICabbage>();
            if (cabbage is null) throw new ArgumentException($"该投射物不是ICabbage: {projectileIdentifier}");
            cabbage.Initialize(direction);
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

        public void RemoveZombie(Zombie zombie)
        {
            ZombieSet.Remove(zombie);
            lastDeadZombiePosition = zombie.transform.position;
            // 触发事件
            this.SendEvent<OnZombieDestroyedEvent>();
        }

        #endregion
    }
}