using System;
using QAssetBundle;
using QFramework;
using TPL.PVZR.Classes.DataClasses.Item.Card;
using TPL.PVZR.Classes.DataClasses.Loot;
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
            switch (lootData.LootType)
            {
                case LootType.Card:
                {
                    return CreateItemView(lootData.CardData);
                }
            }

            throw new NotImplementedException();
        }

        public static GameObject CreateItemView(CardData cardData)
        {
            var go = _cardViewPrefab.Instantiate();
            go.GetComponent<CardView>().Initialize(cardData);
            return go;
        }
    }
}