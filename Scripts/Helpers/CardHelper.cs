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

        public static CardData GetCardData(PlantId id)
        {
            if (_cardsDict.TryGetValue(id, out var cardDefinition))
            {
                return new CardData(cardDefinition);
            }

            throw new ArgumentException("不存在");
        }
        
        public static GameObject CreateSeedOptionController(CardData cardData)
        {
            var go = _seedOptionControllerPrefab.Instantiate();
            go.GetComponent<SeedOptionController>().Initialize(cardData);
            return go;
        }

        #endregion


        private static readonly Dictionary<PlantId, CardDefinition> _cardsDict;
        private static GameObject _seedOptionControllerPrefab;

        static CardHelper()
        {
            ResKit.Init();
            var resLoader = ResLoader.Allocate();


            _cardsDict = new Dictionary<PlantId, CardDefinition>()
            {
                [PlantId.PeaShooter] = resLoader.LoadSync<CardDefinition>(Carddefinition_peashooter_asset.BundleName,
                    Carddefinition_peashooter_asset.CardDefinition_Peashooter),
            };
            //
            _seedOptionControllerPrefab =
                resLoader.LoadSync<GameObject>(Seedoption_prefab.BundleName, Seedoption_prefab.SeedOption);
        }


    }
}