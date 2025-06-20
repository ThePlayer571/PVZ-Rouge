using System;
using System.Collections.Generic;
using QAssetBundle;
using QFramework;
using TPL.PVZR.Classes.GameStuff;
using TPL.PVZR.Core;
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

        public static Plant CreatePlant(PlantId id, Direction2 direction)
        {
            if (_plantDict.TryGetValue(id, out var plantPrefab))
            {
                var go = plantPrefab.Instantiate().GetComponent<Plant>();
                go.Initialize(direction);
                return go;
            }
            else
            {
                throw new ArgumentException($"未考虑的植物类型：{id}");
            }
        }
    }
}