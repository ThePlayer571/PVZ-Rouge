using System;
using System.Collections.Generic;
using QAssetBundle;
using QFramework;
using TPL.PVZR.Classes;
using TPL.PVZR.Classes.DataClasses;
using TPL.PVZR.Classes.DataClasses.Item.Card;
using TPL.PVZR.Classes.DataClasses.Recipe;
using TPL.PVZR.Tools;
using TPL.PVZR.ViewControllers.Others.UI;
using UnityEngine;

namespace TPL.PVZR.Helpers.ClassCreator.Item
{
    public static class CardHelper
    {
        #region Public

        public static CardData CreateCardData(PlantDef plantDef, bool locked = false)
        {
            if (_cardsDict.TryGetValue(plantDef, out var cardDefinition))
            {
                return new CardData(cardDefinition, locked);
            }

            throw new ArgumentException($"未考虑的PlantId和PlantVariant组合：{plantDef.Id}, {plantDef.Variant}");
        }

        public static CardDefinition GetCardDefinition(PlantDef plantDef)
        {
            if (_cardsDict.TryGetValue(plantDef, out var cardDefinition))
            {
                return cardDefinition;
            }

            throw new ArgumentException($"未考虑的PlantId和PlantVariant组合：{plantDef.Id}, {plantDef.Variant}");
        }

        public static SeedOptionController CreateSeedOptionController(CardData cardData)
        {
            var go = _seedOptionControllerPrefab.Instantiate().GetComponent<SeedOptionController>();
            go.Initialize(cardData);
            return go;
        }

        public static SeedController CreateSeedController(SeedData seedData)
        {
            if (seedData == null)
            {
                throw new ArgumentNullException(nameof(seedData));
            }

            var go = _seedControllerPrefab
                .Instantiate(Vector3.zero, Quaternion.identity, ReferenceHelper.LevelGameplayPanel.transform)
                .GetComponent<SeedController>();
            go.Initialize(seedData);
            return go;
        }

        #endregion


        private static readonly Dictionary<PlantDef, CardDefinition> _cardsDict;
        private static readonly GameObject _seedOptionControllerPrefab;
        private static readonly GameObject _seedControllerPrefab;

        static CardHelper()
        {
            ResKit.Init();
            var resLoader = ResLoader.Allocate();

            var list = resLoader.LoadSync<PlantConfigList>(Listconfigs.BundleName, Listconfigs.PlantConfigList)
                .plantConfigs;

            _cardsDict = new Dictionary<PlantDef, CardDefinition>();
            foreach (var config in list)
            {
                _cardsDict[config.def] = config.card;
            }

            _seedControllerPrefab =
                resLoader.LoadSync<GameObject>(Seedcontroller_prefab.BundleName, Seedcontroller_prefab.SeedController);
            _seedOptionControllerPrefab =
                resLoader.LoadSync<GameObject>(Seedoption_prefab.BundleName, Seedoption_prefab.SeedOption);
        }
    }
}