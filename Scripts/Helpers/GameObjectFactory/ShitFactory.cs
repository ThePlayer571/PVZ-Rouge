using System;
using QAssetBundle;
using QFramework;
using TPL.PVZR.Classes.DataClasses_InLevel;
using TPL.PVZR.Classes.DataClasses.Item.Card;
using TPL.PVZR.Tools;
using TPL.PVZR.ViewControllers.Others.UI;
using TPL.PVZR.ViewControllers.UI;
using UnityEngine;

namespace TPL.PVZR.Helpers.New.GameObjectFactory
{
    public static class ShitFactory
    {
        static ShitFactory()
        {
            var resLoader = ResLoader.Allocate();
            _seedOptionControllerPrefab =
                resLoader.LoadSync<GameObject>(Seedoption_prefab.BundleName, Seedoption_prefab.SeedOption);
            _seedControllerPrefab =
                resLoader.LoadSync<GameObject>(Seedcontroller_prefab.BundleName, Seedcontroller_prefab.SeedController);
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
                .Instantiate(Vector3.zero, Quaternion.identity, UIKit.GetPanel<UIChooseSeedPanel>().transform)
                .GetComponent<SeedController>();
            go.Initialize(seedData);
            return go;
        }

        private static readonly GameObject _seedOptionControllerPrefab;
        private static readonly GameObject _seedControllerPrefab;
    }
}