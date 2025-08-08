using System;
using System.Collections.Generic;
using TPL.PVZR.Classes.InfoClasses;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace TPL.PVZR.Classes.ConfigLists
{
    [Serializable]
    public class ZombieConfig
    {
        public ZombieId id;
        public AssetReference prefab;
    }

    [CreateAssetMenu(fileName = "ZombieConfigList", menuName = "PVZR_Config/ZombieConfigList")]
    public class ZombieConfigList : ScriptableObject
    {
        public List<ZombieConfig> Dave;
    }
}