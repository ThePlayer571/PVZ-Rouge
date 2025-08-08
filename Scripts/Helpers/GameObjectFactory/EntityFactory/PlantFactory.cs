using System.Collections.Generic;
using System.Threading.Tasks;
using QFramework;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.Helpers.New.DataReader;
using TPL.PVZR.Helpers.New.Methods;
using TPL.PVZR.Tools;
using TPL.PVZR.ViewControllers.Entities.Plants.Base;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace TPL.PVZR.Helpers.New.GameObjectFactory
{
    public class PlantFactory : ILazyFactory
    {
        private readonly Dictionary<PlantDef, GameObject> _plantPrefabDict = new();
        private readonly Dictionary<PlantDef, Task<GameObject>> _loadingTasks = new();
        private readonly List<AsyncOperationHandle<GameObject>> _handles = new();


        public async Task<Plant> SpawnPlant(PlantDef def, Direction2 direction, Vector2Int cellPos)
        {
            GameObject plantPrefab;
            
            if (_plantPrefabDict.TryGetValue(def, out plantPrefab))
            {
                // 缓存命中，直接使用
            }
            else if (_loadingTasks.TryGetValue(def, out var loadingTask))
            {
                // 正在加载中，等待现有的加载任务完成
                plantPrefab = await loadingTask;
            }
            else
            {
                // 开始新的加载任务
                var plantPrefabAsset = PlantConfigReader.GetPlantPrefab(def);
                var handle = plantPrefabAsset.LoadAssetAsync<GameObject>();
                _handles.Add(handle);
                
                var task = handle.Task;
                _loadingTasks.Add(def, task);
                
                try
                {
                    plantPrefab = await task;
                    _plantPrefabDict.Add(def, plantPrefab);
                }
                finally
                {
                    _loadingTasks.Remove(def);
                }
            }

            var plant = plantPrefab
                .Instantiate(LevelGridHelper.CellToWorldBottom(cellPos), Quaternion.identity)
                .GetComponent<Plant>();
            plant.Initialize(direction);
            return plant;
        }

        public Plant SpawnPlantSync(PlantDef def, Direction2 direction, Vector2Int cellPos)
        {
            if (!_plantPrefabDict.TryGetValue(def, out var plantPrefab))
            {
                var plantPrefabAsset = PlantConfigReader.GetPlantPrefab(def);
                var handle = plantPrefabAsset.LoadAssetAsync<GameObject>();
                _handles.Add(handle);
                handle.WaitForCompletion();
                _plantPrefabDict.Add(def, handle.Result);
                plantPrefab = handle.Result;
            }

            var plant = plantPrefab
                .Instantiate(LevelGridHelper.CellToWorldBottom(cellPos), Quaternion.identity)
                .GetComponent<Plant>();
            plant.Initialize(direction);
            return plant;
        }

        public void ClearCache()
        {
            _plantPrefabDict.Clear();
            _loadingTasks.Clear();
            foreach (var handle in _handles)
            {
                handle.Release();
            }
            _handles.Clear();
        }
    }
}