using System;
using System.Collections.Generic;
using DG.Tweening;
using QAssetBundle;
using QFramework;
using TPL.PVZR.Classes.GameStuff;
using TPL.PVZR.Helpers.Methods;
using TPL.PVZR.Tools;
using TPL.PVZR.Tools.Random;
using TPL.PVZR.ViewControllers;
using TPL.PVZR.ViewControllers.Entities.Plants;
using TPL.PVZR.ViewControllers.Managers;
using UnityEngine;

namespace TPL.PVZR.Helpers.Factory
{
    public static class EntityFactory
    {
        private static ResLoader _resLoader;

        static EntityFactory()
        {
            ResKit.Init();
            _resLoader = ResLoader.Allocate();
        }

        public static class SunFactory
        {
            static SunFactory()
            {
                _sunPrefab = _resLoader.LoadSync<GameObject>(Sun_prefab.BundleName, Sun_prefab.Sun);
            }

            private static GameObject _sunPrefab;

            public static Sun SpawnSunWithJump(Vector2 position)
            {
                var go = _sunPrefab.Instantiate(position, Quaternion.identity).GetComponent<Sun>();
                Vector3 endPos = new Vector3(position.x + (RandomHelper.Default.Range(-0.5f, 0.5f)),
                    position.y + (RandomHelper.Default.Range(0f, 0.2f)), 0);
                go.transform.DOJump(endPos, TestDataManager.Instance.Power, 1, 0.5f);
                return go;
            }
        }

        public static class PlantFactory
        {
            static PlantFactory()
            {
                _plantDict = new Dictionary<PlantId, GameObject>
                {
                    [PlantId.PeaShooter] = _resLoader.LoadSync<GameObject>(Plants.BundleName, Plants.Peashooter),
                    [PlantId.Sunflower] = _resLoader.LoadSync<GameObject>(Plants.BundleName, Plants.Sunflower),
                    [PlantId.Wallnut] = _resLoader.LoadSync<GameObject>(Plants.BundleName, Plants.Wallnut),
                };
            }

            private static Dictionary<PlantId, GameObject> _plantDict;

            public static Plant SpawnPlant(PlantId id, Direction2 direction, Vector2Int cellPos)
            {
                if (_plantDict.TryGetValue(id, out var plantPrefab))
                {
                    var plant = plantPrefab
                        .Instantiate(LevelGridHelper.CellToWorldBottom(cellPos), Quaternion.identity)
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
}