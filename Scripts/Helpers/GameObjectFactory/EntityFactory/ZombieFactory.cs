using System.Collections.Generic;
using System.Threading.Tasks;
using QFramework;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.Helpers.New.DataReader;
using TPL.PVZR.ViewControllers.Entities.Zombies.Base;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace TPL.PVZR.Helpers.New.GameObjectFactory
{
    public class ZombieFactory : ILazyFactory
    {
        private readonly Dictionary<ZombieId, GameObject> _zombiePrefabDict = new();
        private readonly Dictionary<ZombieId, Task<GameObject>> _loadingTasks = new();
        private readonly List<AsyncOperationHandle<GameObject>> _handles = new();
        public HashSet<Zombie> ActiveZombies = new();

        public async Task<Zombie> SpawnZombieAsync(ZombieId id, Vector2 pos, IList<string> paras)
        {
            GameObject zombiePrefab;

            if (_zombiePrefabDict.TryGetValue(id, out zombiePrefab))
            {
                // 缓存命中，直接使用
            }
            else if (_loadingTasks.TryGetValue(id, out var loadingTask))
            {
                // 正在加载中，等待现有的加载任务完成
                zombiePrefab = await loadingTask;
            }
            else
            {
                // 开始新的加载任务
                var zombiePrefabAsset = ZombieConfigReader.GetZombiePrefab(id);
                var handle = zombiePrefabAsset.LoadAssetAsync<GameObject>();
                _handles.Add(handle);

                var task = handle.Task;
                _loadingTasks.Add(id, task);

                try
                {
                    zombiePrefab = await task;
                    _zombiePrefabDict.Add(id, zombiePrefab);
                }
                finally
                {
                    _loadingTasks.Remove(id);
                }
            }

            var zombie = zombiePrefab.Instantiate(pos, Quaternion.identity).GetComponent<Zombie>();
            zombie.Initialize(paras);

            ActiveZombies.Add(zombie);
            return zombie;
        }

        public Zombie SpawnZombie(ZombieId id, Vector2 pos, IList<string> paras)
        {
            if (!_zombiePrefabDict.TryGetValue(id, out var zombiePrefab))
            {
                var zombiePrefabAsset = ZombieConfigReader.GetZombiePrefab(id);
                var handle = zombiePrefabAsset.LoadAssetAsync<GameObject>();
                _handles.Add(handle);
                handle.WaitForCompletion();
                _zombiePrefabDict.Add(id, handle.Result);
                zombiePrefab = handle.Result;
            }

            var zombie = zombiePrefab.Instantiate(pos, Quaternion.identity).GetComponent<Zombie>();
            zombie.Initialize(paras);

            ActiveZombies.Add(zombie);
            return zombie;
        }

        public void RemoveZombie(Zombie zombie)
        {
            zombie.gameObject.DestroySelf();
            ActiveZombies.Remove(zombie);
        }

        public void RemoveAllZombies()
        {
            foreach (var zombie in ActiveZombies)
            {
                zombie.gameObject.DestroySelf();
            }

            ActiveZombies.Clear();
        }

        public void ClearCache()
        {
            _zombiePrefabDict.Clear();
            _loadingTasks.Clear();
            foreach (var handle in _handles)
            {
                handle.Release();
            }

            _handles.Clear();
        }
    }
}