using System;
using System.Collections.Generic;
using TPL.PVZR.Classes.DataClasses.Item.Card;
using TPL.PVZR.Classes.DataClasses.Level;
using UnityEngine;

namespace TPL.PVZR.Classes.DataClasses.Recipe
{
    [Serializable]
    public class PlantConfig
    {
        public PlantDef def;
        public CardDefinition card;
        public GameObject prefab;
    }


    [CreateAssetMenu(fileName = "PlantConfigList", menuName = "PVZR_Config/PlantConfigList", order = 6)]
    public class PlantConfigList : ScriptableObject
    {
        public List<PlantConfig> plantConfigs;
    }
}