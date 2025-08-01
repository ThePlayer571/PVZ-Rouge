using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using QAssetBundle;
using QFramework;
using TPL.PVZR.Classes.ConfigLists;
using TPL.PVZR.Classes.DataClasses_InLevel.ZombieArmor;
using TPL.PVZR.Classes.DataClasses.Recipe;
using UnityEngine.AddressableAssets;

namespace TPL.PVZR.Helpers.New.DataReader
{
    public static class ZombieArmorConfigReader
    {
        #region 数据存储

        private static Dictionary<ZombieArmorId, ZombieArmorDefinition> _zombieArmorDefinitionDict;


        public static async Task Initialize()
        {
            var handle = Addressables.LoadAssetsAsync<ZombieArmorDefinition>("ZombieArmorDefinition", null);
            await handle.Task;
            _zombieArmorDefinitionDict = new Dictionary<ZombieArmorId, ZombieArmorDefinition>();
            foreach (var zombieArmorDefinition in handle.Result)
            {
                _zombieArmorDefinitionDict.Add(zombieArmorDefinition.zombieArmorId, zombieArmorDefinition);
            }

            handle.Release();
        }

        #endregion

        #region 数据读取

        public static ZombieArmorDefinition GetZombieArmorDefinition(ZombieArmorId zombieArmorId)
        {
            if (_zombieArmorDefinitionDict.TryGetValue(zombieArmorId, out var armorDefinition))
            {
                return armorDefinition;
            }

            throw new ArgumentException($"找不到对应的ZombieArmorDefinition: {zombieArmorId}");
        }

        #endregion
    }
}