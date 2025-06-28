using QFramework;
using TMPro;
using TPL.PVZR.Classes.GameStuff;
using TPL.PVZR.CommandEvents.ChooseSeeds;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TPL.PVZR.ViewControllers.Others
{
    public class SeedOptionController : MonoBehaviour, IController, IPointerClickHandler
    {
        public bool IsSelected = false;
        [SerializeField] public Transform View;
        [SerializeField] private Image PlantImage;
        [SerializeField] private TextMeshProUGUI SunpointCostText;

        public void Initialize(CardData cardData)
        {
            this.CardData = cardData;
            PlantImage.sprite = cardData.CardDefinition.PlantSprite;
            SunpointCostText.text = cardData.CardDefinition.SunpointCost.ToString();
        }
        
        
        public CardData CardData;

        public IArchitecture GetArchitecture()
        {
            return PVZRouge.Interface;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!IsSelected)
            {
                this.SendCommand<ChooseSeedCommand>(new ChooseSeedCommand(this));
            }
            else
            {
                this.SendCommand<UnchooseSeedCommand>(new UnchooseSeedCommand(this));
            }
        }
    }
}