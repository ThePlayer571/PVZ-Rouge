using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using QAssetBundle;
using QFramework;
using TPL.PVZR.Classes;
using TPL.PVZR.Classes.ConfigLists;
using TPL.PVZR.Classes.DataClasses_InLevel;
using TPL.PVZR.Classes.DataClasses.Item.Card;
using TPL.PVZR.Classes.DataClasses.Recipe;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.Classes.ZombieAI.Public;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace TPL.PVZR.Helpers.New.DataReader
{
    public static class PlantConfigReader
    {
        #region 数据存储

        private static Dictionary<PlantDef, CardDefinition> _cardDefinitionDict;
        private static Dictionary<PlantDef, AssetReference> _plantPrefabDict;

        public static async Task InitializeAsync()
        {
            var plantConfigListHandle = Addressables.LoadAssetAsync<PlantConfigList>("PlantConfigList");
            await plantConfigListHandle.Task;

            var plantConfigList = plantConfigListHandle.Result;
            // CardDefinition | PlantPrefab
            _cardDefinitionDict = new Dictionary<PlantDef, CardDefinition>();
            _plantPrefabDict = new Dictionary<PlantDef, AssetReference>();
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

            foreach (var config in plantConfigList.GeneralMushroom)
            {
                _cardDefinitionDict[config.def] = config.card;
                _plantPrefabDict[config.def] = config.prefab;
            }

            foreach (var config in plantConfigList.PeaFamily)
            {
                _cardDefinitionDict[config.def] = config.card;
                _plantPrefabDict[config.def] = config.prefab;
            }

            foreach (var config in plantConfigList.PultFamily)
            {
                _cardDefinitionDict[config.def] = config.card;
                _plantPrefabDict[config.def] = config.prefab;
            }
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

        public static AssetReference GetPlantPrefab(PlantDef def)
        {
            if (_plantPrefabDict.TryGetValue(def, out var prefab))
            {
                return prefab;
            }

            throw new ArgumentException($"找不到对应的PlantPrefab: {def.Id}, {def.Variant}");
        }

        public static List<PlantingLocationCondition> GetAllowingPlantingLocations(PlantDef def)
        {
            var key = GetPlantingLocationTypeId(def);
            if (_plantingLocationConditionDict.TryGetValue(key, out var condition))
            {
                return condition;
            }
            else
            {
                var conditions = new List<PlantingLocationCondition>();
                if (key.Item1 != PlantingLocationTypeId.NotSet)
                {
                    conditions.Add(new PlantingLocationCondition(key.Item1));
                }

                if (key.Item2 != PlantingLocationTypeId.NotSet)
                {
                    conditions.Add(new PlantingLocationCondition(key.Item2));
                }

                _plantingLocationConditionDict[key] = conditions;
                return conditions;
            }
        }

        private static Dictionary<(PlantingLocationTypeId, PlantingLocationTypeId), List<PlantingLocationCondition>>
            _plantingLocationConditionDict = new();

        public static (PlantingLocationTypeId, PlantingLocationTypeId) GetPlantingLocationTypeId(PlantDef def)
        {
            return def.Id switch
            {
                PlantId.Flowerpot => (PlantingLocationTypeId.OnPlat, PlantingLocationTypeId.NotSet),
                PlantId.PeaPod => (PlantingLocationTypeId.OnPlatOfNormal, PlantingLocationTypeId.OnSamePlant_OnlyStack),
                PlantId.Pumpkin => (PlantingLocationTypeId.OnPlatOfNormal, PlantingLocationTypeId.OnAnyPlant),
                PlantId.LilyPad or PlantId.TangleKelp => (PlantingLocationTypeId.OnWaterSurface,
                    PlantingLocationTypeId.NotSet),
                PlantId.CoffeeBean => (PlantingLocationTypeId.OnSleepingShroom, PlantingLocationTypeId.NotSet),
                PlantId.GraveBuster => (PlantingLocationTypeId.OnGravestone, PlantingLocationTypeId.NotSet),
                _ => (PlantingLocationTypeId.OnPlatOfNormal, PlantingLocationTypeId.NotSet),
            };
        }

        public static PlacementSlot GetPlacementSlot(PlantDef def)
        {
            return def.Id switch
            {
                PlantId.Pumpkin => PlacementSlot.Overlay,
                PlantId.CoffeeBean => PlacementSlot.Air,
                _ => PlacementSlot.Normal,
            };
        }

        #endregion
    }

    public static class ProjectileConfigReader
    {
        #region 数据存储

        private static Dictionary<ProjectileId, AssetReference> _projectilePrefabDict;

        public static async Task InitializeAsync()
        {
            var projectileConfigListHandle = Addressables.LoadAssetAsync<ProjectileConfigList>("ProjectileConfigList");
            await projectileConfigListHandle.Task;

            var projectileConfigList = projectileConfigListHandle.Result;
            _projectilePrefabDict = new Dictionary<ProjectileId, AssetReference>();
            
            foreach (var config in projectileConfigList.Default)
            {
                _projectilePrefabDict[config.id] = config.prefab;
            }
        }

        #endregion

        #region 数据读取

        public static AssetReference GetProjectilePrefab(ProjectileId id)
        {
            if (_projectilePrefabDict.TryGetValue(id, out var prefab))
            {
                return prefab;
            }

            throw new ArgumentException($"找不到对应的ProjectilePrefab: {id}");
        }

        #endregion
    }
}
