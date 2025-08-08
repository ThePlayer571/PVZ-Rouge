using System;
using System.Collections.Generic;
using TPL.PVZR.Classes.DataClasses.Item.Card;
using TPL.PVZR.Classes.InfoClasses;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace TPL.PVZR.Classes.ConfigLists
{
    [Serializable]
    public class PlantConfig
    {
        public PlantDef def;
        public CardDefinition card;
        public AssetReference prefab;
    }


    [CreateAssetMenu(fileName = "PlantConfigList", menuName = "PVZR_Config/PlantConfigList")]
    public class PlantConfigList : ScriptableObject
    {
        public List<PlantConfig> General;
        public List<PlantConfig> GeneralMushroom;
        public List<PlantConfig> Dave;
        public List<PlantConfig> PeaFamily;
        public List<PlantConfig> PultFamily;
    }
}