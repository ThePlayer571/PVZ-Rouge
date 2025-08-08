using System;
using System.Threading.Tasks;
using QAssetBundle;
using QFramework;
using TPL.PVZR.Classes.DataClasses.Item.Card;
using TPL.PVZR.Classes.DataClasses.Item.PlantBook;
using TPL.PVZR.Classes.DataClasses.Loot;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.Helpers.New.DataReader;
using TPL.PVZR.ViewControllers.Others.UI.ItemView;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace TPL.PVZR.Helpers.New.GameObjectFactory
{
    public static class ItemViewFactory
    {
        public static async Task InitializeAsync()
        {
            var seedSlotViewHandle = Addressables.LoadAssetAsync<GameObject>("SeedSlotView");
            var cardViewHandle = Addressables.LoadAssetAsync<GameObject>("CardView");
            var coinViewHandle = Addressables.LoadAssetAsync<GameObject>("CoinView");
            var plantBookViewHandle = Addressables.LoadAssetAsync<GameObject>("PlantBookView");
            await Task.WhenAll(seedSlotViewHandle.Task
                , cardViewHandle.Task
                , coinViewHandle.Task
                , plantBookViewHandle.Task);
            _seedSlotViewPrefab = seedSlotViewHandle.Result;
            _cardViewPrefab = cardViewHandle.Result;
            _coinViewPrefab = coinViewHandle.Result;
            _plantBookViewPrefab = plantBookViewHandle.Result;
        }

        private static GameObject _cardViewPrefab;
        private static GameObject _coinViewPrefab;
        private static GameObject _plantBookViewPrefab;
        private static GameObject _seedSlotViewPrefab;


        public static GameObject CreateItemView(LootData lootData)
        {
            return lootData.LootType switch
            {
                LootType.Card => CreateItemView(lootData.PlantId, lootData.Locked),
                LootType.Coin => CreateItemView(lootData.CoinAmount),
                LootType.PlantBook => CreateItemView(lootData.PlantBookId),
                LootType.SeedSlot => CreateItemView_SeedSlot(),
                _ => throw new ArgumentException()
            };
        }

        public static GameObject CreateItemView(PlantBookId plantBookId)
        {
            var go = _plantBookViewPrefab.Instantiate();
            go.GetComponent<PlantBookViewController>().Initialize(plantBookId);
            return go;
        }

        public static GameObject CreateItemView(int coinAmount)
        {
            var go = _coinViewPrefab.Instantiate();
            go.GetComponent<CoinViewController>().Initialize(coinAmount);
            return go;
        }

        public static GameObject CreateItemView(CardData cardData)
        {
            var go = _cardViewPrefab.Instantiate();
            go.GetComponent<CardViewController>().Initialize(cardData);
            return go;
        }

        public static GameObject CreateItemView(PlantId plantId, bool locked = false)
        {
            var cardDefinition = PlantConfigReader.GetCardDefinition(plantId.ToDef());
            var go = _cardViewPrefab.Instantiate();
            go.GetComponent<CardViewController>().Initialize(cardDefinition, locked);
            return go;
        }

        public static GameObject CreateItemView_SeedSlot()
        {
            return _seedSlotViewPrefab.Instantiate();
        }
    }
}