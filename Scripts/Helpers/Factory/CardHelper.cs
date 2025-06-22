using System;
using System.Collections.Generic;
using QAssetBundle;
using QFramework;
using TPL.PVZR.ViewControllers.Others;
using UnityEngine;

namespace TPL.PVZR.Classes.GameStuff
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

        public static SeedController CreateSeedController(CardData cardData, int index)
        {
            var go = _seedControllerPrefab.Instantiate().GetComponent<SeedController>();
            go.Initialize(cardData, index);
            return go;
        }

        #endregion


        private static readonly Dictionary<PlantId, CardDefinition> _cardsDict;
        private static GameObject _seedOptionControllerPrefab;
        private static GameObject _seedControllerPrefab;

        static CardHelper()
        {
            ResKit.Init();
            var resLoader = ResLoader.Allocate();


            _cardsDict = new Dictionary<PlantId, CardDefinition>()
            {
                [PlantId.PeaShooter] = resLoader.LoadSync<CardDefinition>(Carddefinition_peashooter_asset.BundleName,
                    Carddefinition_peashooter_asset.CardDefinition_Peashooter),
            };
            _seedControllerPrefab =
                resLoader.LoadSync<GameObject>(Seedcontroller_prefab.BundleName, Seedcontroller_prefab.SeedController);
            _seedOptionControllerPrefab =
                resLoader.LoadSync<GameObject>(Seedoption_prefab.BundleName, Seedoption_prefab.SeedOption);
        }
    }
}