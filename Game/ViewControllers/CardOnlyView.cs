using System;
using QFramework;
using UnityEngine;
using UnityEngine.UI;

namespace TPL.PVZR
{
    public partial class CardOnlyView : ViewController,IController
    {
        public IArchitecture GetArchitecture()
        {
            return PVZRouge.Interface;
        }
        public CardDataSO cardData;
        public void Init(CardDataSO cardDataSO)
        {
            Plant.sprite = cardDataSO.seedData.plantSprite;
            SunText.text = cardDataSO.seedData.sunpointCost.ToString();
        }

        public static GameObject Create(CardDataSO cardData)
        {
            var go = ResLoader.Allocate().LoadSync<GameObject>("CardOnlyView").Instantiate();
            go.GetComponent<CardOnlyView>().Init(cardData);
            return go;
        }
    }
}