using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using QFramework;
using TPL.PVZR.EntityZombie;
using UnityEngine;

namespace TPL.PVZR
{
    public partial class ZombieSpawnData
    {
        public ZombieIdentifier zombieIdentifier;
        public float weight;
        public float value;
    }

    public partial class ZombieSpawnData
    {
        private static Dictionary<ZombieIdentifier, ZombieSpawnData> ZombieSpawnDataDict = new();

        public static ZombieSpawnData GetDefaultData(ZombieIdentifier zombieIdentifier)
        {
            return ZombieSpawnDataDict.TryGetValue(zombieIdentifier, out var data)
                ? data
                : ZombieSpawnDataDict[ZombieIdentifier.NormalZombie];
        }

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
    }
}