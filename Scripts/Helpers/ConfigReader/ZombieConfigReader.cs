using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TPL.PVZR.Classes.ConfigLists;
using TPL.PVZR.Classes.InfoClasses;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace TPL.PVZR.Helpers.New.DataReader
{
    public static class ZombieConfigReader
    {
        #region 数据存储

        private static Dictionary<ZombieId, ZombieConfig> _zombieConfigs;


        public static async Task InitializeAsync()
        {
            var zombieConfigListHandle = Addressables.LoadAssetAsync<ZombieConfigList>("ZombieConfigList");
            await zombieConfigListHandle.Task;
            var zombieConfigList = zombieConfigListHandle.Result;
//
            _zombieConfigs = new Dictionary<ZombieId, ZombieConfig>();
            foreach (var config in zombieConfigList.Dave)
            {
                _zombieConfigs[config.id] = config;
            }
        }

        #endregion

        #region 数据读取

        public static AssetReference GetZombiePrefab(ZombieId zombieId)
        {
            if (_zombieConfigs.TryGetValue(zombieId, out var config))
            {
                return config.prefab;
            }

            throw new ArgumentException($"找不到对应的ZombiePrefab: {zombieId}");
        }

        #endregion
    }
}