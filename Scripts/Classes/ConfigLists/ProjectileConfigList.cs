using System;
using System.Collections.Generic;
using TPL.PVZR.Classes.InfoClasses;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace TPL.PVZR.Classes.ConfigLists
{
    [Serializable]
    public class ProjectileConfig
    {
        public ProjectileId id;
        public AssetReference prefab;
    }

    [CreateAssetMenu(menuName = "ProjectileConfigList", fileName = "PVZR_Config/ProjectileConfigList", order = 0)]
    public class ProjectileConfigList : ScriptableObject
    {
        public List<ProjectileConfig> Default;
    }
}