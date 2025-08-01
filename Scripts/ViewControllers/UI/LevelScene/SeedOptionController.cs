using QFramework;
using TPL.PVZR.Classes.DataClasses.Item.Card;
using TPL.PVZR.CommandEvents.Level_ChooseSeeds;
using TPL.PVZR.Models;
using TPL.PVZR.ViewControllers.Others.UI.ItemView;
using TPL.PVZR.ViewControllers.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TPL.PVZR.ViewControllers.Others.UI
{
    public class SeedOptionController : MonoBehaviour, IController, IPointerClickHandler
    {
        [SerializeField] public CardViewController cardView;
        private IGameModel _GameModel;
        private UIChooseSeedPanel ChooseSeedPanel;

        public bool IsSelected = false;
        public CardData CardData;

        public void Initialize(CardData cardData)
        {
            this.CardData = cardData;
            cardView.Initialize(cardData);
        }


        private void Awake()
        {
            this._GameModel = this.GetModel<IGameModel>();
            this.ChooseSeedPanel = UIKit.GetPanel<UIChooseSeedPanel>();
        }

        public IArchitecture GetArchitecture()
        {
            return PVZRouge.Interface;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!IsSelected)
            {
                if (ChooseSeedPanel.chosenSeedOptions.Count >=
                    _GameModel.GameData.InventoryData.SeedSlotCount.Value) return;
                this.SendCommand<ChooseSeedCommand>(new ChooseSeedCommand(this));
            }
            else
            {
                this.SendCommand<UnchooseSeedCommand>(new UnchooseSeedCommand(this));
            }
        }
    }
}