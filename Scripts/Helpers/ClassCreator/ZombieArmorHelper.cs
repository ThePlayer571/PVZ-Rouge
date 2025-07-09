using System;
using System.Collections.Generic;
using QAssetBundle;
using QFramework;
using TPL.PVZR.Classes.DataClasses.Attack;
using TPL.PVZR.Classes.DataClasses.ZombieArmor;

namespace TPL.PVZR.Helpers.ClassCreator
{
    public static class ZombieArmorHelper
    {
        private static Dictionary<ZombieArmorId, ZombieArmorDefinition> _zombieArmorDict;

        static ZombieArmorHelper()
        {
            var resLoader = ResLoader.Allocate();
            _zombieArmorDict = new Dictionary<ZombieArmorId, ZombieArmorDefinition>()
            {
                [ZombieArmorId.TrafficCone] = resLoader.LoadSync<ZombieArmorDefinition>(
                    Zombiearmordefinition.BundleName, Zombiearmordefinition.ZombieArmorDefinition_TrafficCone),
            };
        }

        public static ZombieArmorData CreateZombieArmorData(ZombieArmorId id)
        {
            if (_zombieArmorDict.TryGetValue(id, out var zombieArmorDefinition))
            {
                return new ZombieArmorData(zombieArmorDefinition);
            }
            else
            {
                throw new ArgumentException($"未考虑的ZombieArmor类型: {id}");
            }
        }
    }
}