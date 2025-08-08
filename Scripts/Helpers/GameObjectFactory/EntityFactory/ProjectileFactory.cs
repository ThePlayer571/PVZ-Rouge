using System.Collections.Generic;
using System.Threading.Tasks;
using QFramework;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.Helpers.New.DataReader;
using TPL.PVZR.ViewControllers.Entities.Projectiles;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace TPL.PVZR.Helpers.New.GameObjectFactory
{
    public class ProjectileFactory : ILazyFactory
    {
        private readonly Dictionary<ProjectileId, GameObject> _projectilePrefabDict = new();
        private readonly Dictionary<ProjectileId, Task<GameObject>> _loadingTasks = new();
        private readonly List<AsyncOperationHandle<GameObject>> _handles = new();

        public async Task<Projectile> CreatePea(ProjectileId id, Vector2 direction, Vector2 pos)
        {
            GameObject projectilePrefab;
            
            if (_projectilePrefabDict.TryGetValue(id, out projectilePrefab))
            {
                // 缓存命中，直接使用
            }
            else if (_loadingTasks.TryGetValue(id, out var loadingTask))
            {
                // 正在加载中，等待现有的加载任务完成
                projectilePrefab = await loadingTask;
            }
            else
            {
                // 开始新的加载任务
                var projectilePrefabAsset = ProjectileConfigReader.GetProjectilePrefab(id);
                var handle = projectilePrefabAsset.LoadAssetAsync<GameObject>();
                _handles.Add(handle);
                
                var task = handle.Task;
                _loadingTasks.Add(id, task);
                
                try
                {
                    projectilePrefab = await task;
                    _projectilePrefabDict.Add(id, projectilePrefab);
                }
                finally
                {
                    _loadingTasks.Remove(id);
                }
            }

            var peaLikeInit = projectilePrefab.GetComponent<IPeaLikeInit>();
            if (peaLikeInit == null)
                throw new System.Exception($"Prefab未实现IPeaLikeInit接口: {projectilePrefab.name}");

            var go = projectilePrefab.Instantiate(pos, Quaternion.identity).GetComponent<IPeaLikeInit>();
            go.Initialize(direction.normalized);

            // 返回Projectile基类
            var projectile = go as Projectile;
            return projectile;
        }

        public Projectile CreatePeaSync(ProjectileId id, Vector2 direction, Vector2 pos)
        {
            if (!_projectilePrefabDict.TryGetValue(id, out var projectilePrefab))
            {
                var projectilePrefabAsset = ProjectileConfigReader.GetProjectilePrefab(id);
                var handle = projectilePrefabAsset.LoadAssetAsync<GameObject>();
                _handles.Add(handle);
                handle.WaitForCompletion();
                _projectilePrefabDict.Add(id, handle.Result);
                projectilePrefab = handle.Result;
            }

            var peaLikeInit = projectilePrefab.GetComponent<IPeaLikeInit>();
            if (peaLikeInit == null)
                throw new System.Exception($"Prefab未实现IPeaLikeInit接口: {projectilePrefab.name}");

            var go = projectilePrefab.Instantiate(pos, Quaternion.identity).GetComponent<IPeaLikeInit>();
            go.Initialize(direction.normalized);

            // 返回Projectile基类
            var projectile = go as Projectile;
            return projectile;
        }

        public void ClearCache()
        {
            _projectilePrefabDict.Clear();
            _loadingTasks.Clear();
            foreach (var handle in _handles)
            {
                handle.Release();
            }
            _handles.Clear();
        }
    }
}