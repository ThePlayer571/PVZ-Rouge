using System;
using QAssetBundle;
using QFramework;
using TPL.PVZR.Classes;
using TPL.PVZR.Classes.DataClasses.Item.Card;
using TPL.PVZR.Classes.DataClasses.Item.PlantBook;
using TPL.PVZR.Classes.DataClasses.Loot;
using TPL.PVZR.Helpers.ClassCreator.Item;
using TPL.PVZR.ViewControllers.Others.UI.ItemView;
using UnityEngine;

namespace TPL.PVZR.Helpers
{
    public static class ItemViewFactory
    {
        static ItemViewFactory()
        {
            var resLoader = ResLoader.Allocate();

            _cardViewPrefab = resLoader.LoadSync<GameObject>(Items.BundleName, Items.CardView);
            _coinViewPrefab = resLoader.LoadSync<GameObject>(Items.BundleName, Items.CoinView);
            _plantBookViewPrefab = resLoader.LoadSync<GameObject>(Items.BundleName, Items.PlantBookView);
        }

        private static readonly GameObject _cardViewPrefab;
        private static readonly GameObject _coinViewPrefab;
        private static readonly GameObject _plantBookViewPrefab;


        public static GameObject CreateItemView(LootData lootData)
        {
            return lootData.LootType switch
            {
                LootType.Card => CreateItemView(lootData.PlantId),
                LootType.Coin => CreateItemView(lootData.CoinAmount),
                LootType.PlantBook => CreateItemView(lootData.PlantBookId),
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

        public static GameObject CreateItemView(PlantId plantId)
        {
            var cardDefinition = CardHelper.GetCardDefinition(PlantBookHelper.GetPlantDef(plantId));
            var go = _cardViewPrefab.Instantiate();
            go.GetComponent<CardViewController>().Initialize(cardDefinition);
            return go;
        }
    }
}