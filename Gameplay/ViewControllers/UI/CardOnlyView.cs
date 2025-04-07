using QFramework;
using TPL.PVZR.Architecture;
using TPL.PVZR.Gameplay.Data;
using UnityEngine;
using UnityEngine.Serialization;

namespace TPL.PVZR.Gameplay.ViewControllers.UI
{
    public partial class CardOnlyView : ViewController,IController
    {
        public IArchitecture GetArchitecture()
        {
            return PVZRouge.Interface;
        }
        [FormerlySerializedAs("cardData")] public CardSO cardSO;
        public void Init(CardSO cardSO)
        {
            Plant.sprite = cardSO.seedSO.plantSprite;
            SunText.text = cardSO.seedSO.sunpointCost.ToString();
        }

        public static GameObject Create(CardSO cardSO)
        {
            var go = ResLoader.Allocate().LoadSync<GameObject>("CardOnlyView").Instantiate();
            go.GetComponent<CardOnlyView>().Init(cardSO);
            return go;
        }
    }
}