using System;
using System.Collections.Generic;
using QAssetBundle;
using QFramework;
using TPL.PVZR.Classes.ConfigLists;
using TPL.PVZR.Classes.DataClasses_InLevel.ZombieArmor;
using TPL.PVZR.Classes.DataClasses.Recipe;

namespace TPL.PVZR.Helpers.New.DataReader
{
    public static class ZombieArmorConfigReader
    {
        #region 数据存储

        private static readonly Dictionary<ZombieArmorId, ZombieArmorDefinition> _zombieArmorDefinitionDict;

        static ZombieArmorConfigReader()
        {
            ResKit.Init();
            var _resLoader = ResLoader.Allocate();
            var zombieArmorConfigList = _resLoader
                .LoadSync<ZombieArmorDefinitionList>(Configlist.BundleName, Configlist.ZombieArmorDefinitionList)
                .zombieArmorDefinitionList;

            // ZombieArmorDefinition
            _zombieArmorDefinitionDict = new Dictionary<ZombieArmorId, ZombieArmorDefinition>();
            foreach (var config in zombieArmorConfigList)
            {
                _zombieArmorDefinitionDict.Add(config.zombieArmorId, config.zombieArmorDefinition);
            }
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