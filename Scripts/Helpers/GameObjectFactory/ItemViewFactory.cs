using System;
using QAssetBundle;
using QFramework;
using TPL.PVZR.Classes.DataClasses.Item.Card;
using TPL.PVZR.Classes.DataClasses.Item.PlantBook;
using TPL.PVZR.Classes.DataClasses.Loot;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.Helpers.New.DataReader;
using TPL.PVZR.ViewControllers.Others.UI.ItemView;
using UnityEngine;

namespace TPL.PVZR.Helpers.New.GameObjectFactory
{
    public static class ItemViewFactory
    {
        static ItemViewFactory()
        {
            var resLoader = ResLoader.Allocate();

            _cardViewPrefab = resLoader.LoadSync<GameObject>(Items.BundleName, Items.CardView);
            _coinViewPrefab = resLoader.LoadSync<GameObject>(Items.BundleName, Items.CoinView);
            _plantBookViewPrefab = resLoader.LoadSync<GameObject>(Items.BundleName, Items.PlantBookView);
            _seedSlotViewPrefab = resLoader.LoadSync<GameObject>(Items.BundleName, Items.SeedSlotView);


            resLoader.Recycle2Cache();
        }

        private static readonly GameObject _cardViewPrefab;
        private static readonly GameObject _coinViewPrefab;
        private static readonly GameObject _plantBookViewPrefab;
        private static readonly GameObject _seedSlotViewPrefab;


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