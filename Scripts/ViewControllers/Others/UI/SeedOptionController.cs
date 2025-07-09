using QFramework;
using TMPro;
using TPL.PVZR.Classes.DataClasses.Item.Card;
using TPL.PVZR.CommandEvents.Level_ChooseSeeds;
using TPL.PVZR.Models;
using TPL.PVZR.Tools;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TPL.PVZR.ViewControllers.Others.UI
{
    public class SeedOptionController : MonoBehaviour, IController, IPointerClickHandler
    {
        [SerializeField] public Transform View;
        [SerializeField] private Image PlantImage;
        [SerializeField] private TextMeshProUGUI SunpointCostText;
        public IGameModel _GameModel;
        
        public bool IsSelected = false;
        public CardData CardData;

        public void Initialize(CardData cardData)
        {
            this.CardData = cardData;
            PlantImage.sprite = cardData.CardDefinition.PlantSprite;
            SunpointCostText.text = cardData.CardDefinition.SunpointCost.ToString();
        }
        
        

        private void Awake()
        {
            this._GameModel = this.GetModel<IGameModel>();
        }

        public IArchitecture GetArchitecture()
        {
            return PVZRouge.Interface;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!IsSelected)
            {
                if (ReferenceHelper.ChooseSeedPanel.chosenSeedOptions.Count >=
                    _GameModel.GameData.InventoryData.SeedSlotCount) return;
                this.SendCommand<ChooseSeedCommand>(new ChooseSeedCommand(this));
            }
            else
            {
                this.SendCommand<UnchooseSeedCommand>(new UnchooseSeedCommand(this));
            }
        }
    }
}