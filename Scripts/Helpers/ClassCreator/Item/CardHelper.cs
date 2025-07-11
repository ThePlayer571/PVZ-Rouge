using System;
using System.Collections.Generic;
using QAssetBundle;
using QFramework;
using TPL.PVZR.Classes;
using TPL.PVZR.Classes.DataClasses;
using TPL.PVZR.Classes.DataClasses.Item.Card;
using TPL.PVZR.Tools;
using TPL.PVZR.ViewControllers.Others.UI;
using UnityEngine;

namespace TPL.PVZR.Helpers.ClassCreator.Item
{
    public static class CardHelper
    {
        #region Public

        public static CardData CreateCardData(PlantId id, PlantVariant variant = PlantVariant.V0, bool locked = false)
        {
            if (_cardsDict.TryGetValue((id, variant), out var cardDefinition))
            {
                return new CardData(cardDefinition, locked);
            }

            throw new ArgumentException($"未考虑的PlantId和PlantVariant组合：{id}, {variant}");
        }

        public static CardDefinition GetCardDefinition(PlantId id, PlantVariant variant = PlantVariant.V0)
        {
            if (_cardsDict.TryGetValue((id, variant), out var cardDefinition))
            {
                return cardDefinition;
            }

            throw new ArgumentException($"未考虑的PlantId和PlantVariant组合：{id}, {variant}");
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


        private static readonly Dictionary<(PlantId, PlantVariant), CardDefinition> _cardsDict;
        private static readonly GameObject _seedOptionControllerPrefab;
        private static readonly GameObject _seedControllerPrefab;

        static CardHelper()
        {
            ResKit.Init();
            var resLoader = ResLoader.Allocate();

            _cardsDict = new Dictionary<(PlantId, PlantVariant), CardDefinition>()
            {
                [(PlantId.PeaShooter, PlantVariant.V0)] = resLoader.LoadSync<CardDefinition>(Carddefinition.BundleName,
                    Carddefinition.CardDefinition_Peashooter),
                [(PlantId.Sunflower, PlantVariant.V0)] = resLoader.LoadSync<CardDefinition>(Carddefinition.BundleName,
                    Carddefinition.CardDefinition_Sunflower),
                [(PlantId.Wallnut, PlantVariant.V0)] = resLoader.LoadSync<CardDefinition>(Carddefinition.BundleName,
                    Carddefinition.CardDefinition_Wallnut),
                [(PlantId.Flowerpot, PlantVariant.V0)] = resLoader.LoadSync<CardDefinition>(Carddefinition.BundleName,
                    Carddefinition.CardDefinition_Flowerpot),
                [(PlantId.SnowPea, PlantVariant.V0)] = resLoader.LoadSync<CardDefinition>(Carddefinition.BundleName,
                    Carddefinition.CardDefinition_SnowPea),
                [(PlantId.Marigold, PlantVariant.V0)] = resLoader.LoadSync<CardDefinition>(Carddefinition.BundleName,
                    Carddefinition.CardDefinition_Marigold),
            };
            _seedControllerPrefab =
                resLoader.LoadSync<GameObject>(Seedcontroller_prefab.BundleName, Seedcontroller_prefab.SeedController);
            _seedOptionControllerPrefab =
                resLoader.LoadSync<GameObject>(Seedoption_prefab.BundleName, Seedoption_prefab.SeedOption);
        }
    }
}