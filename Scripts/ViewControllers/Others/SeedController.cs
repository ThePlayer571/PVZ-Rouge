using QFramework;
using TMPro;
using TPL.PVZR.Classes.GameStuff;
using UnityEngine;
using UnityEngine.UI;

namespace TPL.PVZR.ViewControllers.Others
{
    public class SeedController : MonoBehaviour, IController
    {
        [SerializeField] private Image PlantImage;
        [SerializeField] private TextMeshProUGUI SunpointCostText;
        [SerializeField] private Image GrayMask;
        [SerializeField] private Image BlackMask;

        public CardData CardData;

        public void Initialize(CardData cardData)
        {
            this.CardData = cardData;
            PlantImage.sprite = cardData.CardDefinition.PlantSprite;
            SunpointCostText.text = cardData.CardDefinition.SunpointCost.ToString();
        }


        public IArchitecture GetArchitecture()
        {
            return PVZRouge.Interface;
        }
    }
}