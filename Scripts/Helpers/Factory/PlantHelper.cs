using System;
using System.Collections.Generic;
using QAssetBundle;
using QFramework;
using TPL.PVZR.Classes.GameStuff;
using TPL.PVZR.Core;
using TPL.PVZR.Helpers.Methods;
using TPL.PVZR.ViewControllers.Entities.Plants;
using TPL.PVZR.ViewControllers.Entities.Projectiles;
using UnityEngine;

namespace TPL.PVZR.Helpers
{
    public static class PlantHelper
    {
        static PlantHelper()
        {
            ResKit.Init();
            var resLoader = ResLoader.Allocate();

            _plantDict = new Dictionary<PlantId, GameObject>
            {
                [PlantId.PeaShooter] =
                    resLoader.LoadSync<GameObject>(Peashooter_prefab.BundleName, Peashooter_prefab.Peashooter),
            };
        }

        private static Dictionary<PlantId, GameObject> _plantDict;

        public static Plant SpawnPlant(PlantId id, Direction2 direction, Vector2Int position)
        {
            if (_plantDict.TryGetValue(id, out var plantPrefab))
            {
                var plant = plantPrefab
                    .Instantiate(LevelGridHelper.CellToWorldBottom(position), Quaternion.identity)
                    .GetComponent<Plant>();
                plant.Initialize(direction);
                return plant;
            }
            else
            {
                throw new ArgumentException($"未考虑的植物类型：{id}");
            }
        }
    }
}