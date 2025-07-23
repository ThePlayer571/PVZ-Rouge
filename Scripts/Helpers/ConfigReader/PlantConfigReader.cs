using System;
using System.Collections.Generic;
using QAssetBundle;
using QFramework;
using TPL.PVZR.Classes;
using TPL.PVZR.Classes.ConfigLists;
using TPL.PVZR.Classes.DataClasses_InLevel;
using TPL.PVZR.Classes.DataClasses.Item.Card;
using TPL.PVZR.Classes.DataClasses.Recipe;
using TPL.PVZR.Classes.InfoClasses;
using UnityEngine;

namespace TPL.PVZR.Helpers.New.DataReader
{
    public static class PlantConfigReader
    {
        #region 数据存储

        private static readonly Dictionary<PlantDef, CardDefinition> _cardDefinitionDict;
        private static readonly Dictionary<PlantDef, GameObject> _plantPrefabDict;


        static PlantConfigReader()
        {
            ResKit.Init();
            var _resLoader = ResLoader.Allocate();
            var plantConfigList =
                _resLoader.LoadSync<PlantConfigList>(Configlist.BundleName, Configlist.PlantConfigList);

            // CardDefinition | PlantPrefab
            _cardDefinitionDict = new Dictionary<PlantDef, CardDefinition>();
            _plantPrefabDict = new Dictionary<PlantDef, GameObject>();
            foreach (var config in plantConfigList.Dave)
            {
                _cardDefinitionDict[config.def] = config.card;
                _plantPrefabDict[config.def] = config.prefab;
            }

            foreach (var config in plantConfigList.General)
            {
                _cardDefinitionDict[config.def] = config.card;
                _plantPrefabDict[config.def] = config.prefab;
            }

            foreach (var config in plantConfigList.PeaFamily)
            {
                _cardDefinitionDict[config.def] = config.card;
                _plantPrefabDict[config.def] = config.prefab;
            }

            _resLoader.Recycle2Cache();
        }

        #endregion

        #region 数据读取

        public static CardDefinition GetCardDefinition(PlantDef plantDef)
        {
            if (_cardDefinitionDict.TryGetValue(plantDef, out var cardDefinition))
            {
                return cardDefinition;
            }

            throw new ArgumentException($"找不到对应的CardDefinition: {plantDef.Id}, {plantDef.Variant}");
        }

        public static GameObject GetPlantPrefab(PlantDef def)
        {
            if (_plantPrefabDict.TryGetValue(def, out var prefab))
            {
                return prefab;
            }

            throw new ArgumentException($"找不到对应的PlantPrefab: {def.Id}, {def.Variant}");
        }

        public static AllowedPlantingLocation GetAllowedPlantingLocation(PlantDef def)
        {
            return def.Id switch
            {
                PlantId.Flowerpot => AllowedPlantingLocation.OnPlat,
                PlantId.PeaPod => AllowedPlantingLocation.OnSamePlantAndDirt,
                _ => AllowedPlantingLocation.OnDirt,
            };
        }

        public static PlacementSlotInCell GetPlacementSlotInCell(PlantDef def)
        {
            return def.Id switch
            {
                _ => PlacementSlotInCell.Normal,
            };
        }

        #endregion
    }
}