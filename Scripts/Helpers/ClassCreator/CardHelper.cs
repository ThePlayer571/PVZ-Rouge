using System;
using System.Collections.Generic;
using QAssetBundle;
using QFramework;
using TPL.PVZR.Classes;
using TPL.PVZR.Classes.DataClasses;
using TPL.PVZR.Classes.DataClasses.Card;
using TPL.PVZR.Tools;
using TPL.PVZR.ViewControllers.Others;
using UnityEngine;

namespace TPL.PVZR.Helpers.ClassCreator
{
    public static class CardHelper
    {
        #region Public

        public static CardData CreateCardData(PlantId id)
        {
            if (_cardsDict.TryGetValue(id, out var cardDefinition))
            {
                return new CardData(cardDefinition);
            }

            throw new ArgumentException($"未考虑的PlantId：{id}");
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


        private static readonly Dictionary<PlantId, CardDefinition> _cardsDict;
        private static readonly GameObject _seedOptionControllerPrefab;
        private static readonly GameObject _seedControllerPrefab;

        static CardHelper()
        {
            ResKit.Init();
            var resLoader = ResLoader.Allocate();


            _cardsDict = new Dictionary<PlantId, CardDefinition>()
            {
                [PlantId.PeaShooter] = resLoader.LoadSync<CardDefinition>(Carddefinition.BundleName,
                    Carddefinition.CardDefinition_Peashooter),
                [PlantId.Sunflower] = resLoader.LoadSync<CardDefinition>(Carddefinition.BundleName,
                    Carddefinition.CardDefinition_Sunflower),
                [PlantId.Wallnut] = resLoader.LoadSync<CardDefinition>(Carddefinition.BundleName,
                    Carddefinition.CardDefinition_Wallnut),
                [PlantId.Flowerpot] = resLoader.LoadSync<CardDefinition>(Carddefinition.BundleName,
                    Carddefinition.CardDefinition_Flowerpot),
                [PlantId.SnowPea] = resLoader.LoadSync<CardDefinition>(Carddefinition.BundleName,
                    Carddefinition.CardDefinition_SnowPea),
            };
            _seedControllerPrefab =
                resLoader.LoadSync<GameObject>(Seedcontroller_prefab.BundleName, Seedcontroller_prefab.SeedController);
            _seedOptionControllerPrefab =
                resLoader.LoadSync<GameObject>(Seedoption_prefab.BundleName, Seedoption_prefab.SeedOption);
        }
    }
}