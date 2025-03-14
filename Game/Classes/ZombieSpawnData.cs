using System.Collections;
using System.Collections.Generic;
using TPL.PVZR.EntityZombie;

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
        private static Dictionary<ZombieIdentifier, ZombieSpawnData> ZombieSpawnDataDict;

        public static ZombieSpawnData GetDefaultData(ZombieIdentifier zombieIdentifier)
        {
            return ZombieSpawnDataDict.TryGetValue(zombieIdentifier, out var data)
                ? data
                : ZombieSpawnDataDict[ZombieIdentifier.NormalZombie];
        }

        static ZombieSpawnData()
        {
            ZombieSpawnDataDict = new Dictionary<ZombieIdentifier, ZombieSpawnData>
            {
                [ZombieIdentifier.NormalZombie] = new ZombieSpawnData
                    { zombieIdentifier = ZombieIdentifier.NormalZombie, weight = 1000, value = 10 },
                [ZombieIdentifier.ConeheadZombie] = new ZombieSpawnData
                    { zombieIdentifier = ZombieIdentifier.ConeheadZombie, weight = 400, value = 15 },
                [ZombieIdentifier.BucketZombie] = new ZombieSpawnData
                    { zombieIdentifier = ZombieIdentifier.BucketZombie, weight = 200, value = 30 },
                [ZombieIdentifier.ScreenDoorZombie] = new ZombieSpawnData
                    { zombieIdentifier = ZombieIdentifier.ScreenDoorZombie, weight = 300, value = 30 }
            };
        }
    }
}