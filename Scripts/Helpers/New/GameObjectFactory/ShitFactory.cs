using System;
using TPL.PVZR.Classes.DataClasses;
using TPL.PVZR.Classes.DataClasses.Item.Card;
using TPL.PVZR.Tools;
using TPL.PVZR.ViewControllers.Others.UI;
using UnityEngine;
using QFramework;

namespace TPL.PVZR.Helpers
{
    public static class ShitFactory
    {
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

        private static readonly GameObject _seedOptionControllerPrefab;
        private static readonly GameObject _seedControllerPrefab;
    }
}