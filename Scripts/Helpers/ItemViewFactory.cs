using System;
using QAssetBundle;
using QFramework;
using TPL.PVZR.Classes;
using TPL.PVZR.Classes.DataClasses.Item.Card;
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
        }

        private static readonly GameObject _cardViewPrefab;

        public static GameObject CreateItemView(LootData lootData)
        {
            return lootData.LootType switch
            {
                LootType.Card => CreateItemView(lootData.CardData),
                _ => throw new ArgumentException()
            };
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