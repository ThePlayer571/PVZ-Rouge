using System;
using System.Threading.Tasks;
using QAssetBundle;
using QFramework;
using TPL.PVZR.Classes.DataClasses_InLevel;
using TPL.PVZR.Classes.DataClasses.Item.Card;
using TPL.PVZR.Tools;
using TPL.PVZR.ViewControllers.Others.UI;
using TPL.PVZR.ViewControllers.UI;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace TPL.PVZR.Helpers.New.GameObjectFactory
{
    public static class ShitFactory
    {
        private static GameObject _seedOptionControllerPrefab;
        private static GameObject _seedControllerPrefab;

        public static async Task InitializeAsync()
        {
            var handle_seedOptionController = Addressables.LoadAssetAsync<GameObject>("SeedOption");
            var handle_seedController = Addressables.LoadAssetAsync<GameObject>("SeedController");
            await Task.WhenAll(handle_seedOptionController.Task, handle_seedController.Task);

            _seedOptionControllerPrefab = handle_seedOptionController.Result;
            _seedControllerPrefab = handle_seedController.Result;
        }

        public static SeedOptionController CreateSeedOptionController(CardData cardData)
        {
            var go = _seedOptionControllerPrefab.Instantiate().GetComponent<SeedOptionController>();
            go.Initialize(cardData);
            return go;
        }

        public static SeedController CreateSeedController(SeedData seedData)
        {
            var go = _seedControllerPrefab.Instantiate().GetComponent<SeedController>();
            go.Initialize(seedData);
            return go;
        }
    }
}