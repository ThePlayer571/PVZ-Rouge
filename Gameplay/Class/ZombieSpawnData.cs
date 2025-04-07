using System.Collections.Generic;
using Newtonsoft.Json;
using QFramework;
using TPL.PVZR.Gameplay.Entities;
using UnityEngine;

namespace TPL.PVZR.Gameplay.Class
{
    public class ZombieSpawnData
    { 
        # region 属性
        public ZombieIdentifier zombieIdentifier;
        public float weight;
        public float value;
        # endregion
        
        # region 静态方法
        public static ZombieSpawnData GetDefaultData(ZombieIdentifier zombieIdentifier)
        {
            return ZombieSpawnDataDict.TryGetValue(zombieIdentifier, out var data)
                ? data
                : ZombieSpawnDataDict[ZombieIdentifier.NormalZombie];
        }
        # endregion
        
        # region 私有
        private static Dictionary<ZombieIdentifier, ZombieSpawnData> ZombieSpawnDataDict = new();


        static ZombieSpawnData()
        {
            var json = ResLoader.Allocate().LoadSync<TextAsset>("ZombieSpawnDataJson");
            var ZombieSpawnDataJsonList = JsonConvert.DeserializeObject<List<ZombieSpawnDataJson>>(json.text);

            foreach (var each in ZombieSpawnDataJsonList)
            {
                ZombieSpawnDataDict[each.id] =
                    new ZombieSpawnData
                    {
                        zombieIdentifier = each.id,
                        weight = each.weight,
                        value = each.value
                    };
            }
        }
        private struct ZombieSpawnDataJson
        {
            public ZombieIdentifier id;
            public float weight;
            public float value;
        }
        # endregion
        
    }

}