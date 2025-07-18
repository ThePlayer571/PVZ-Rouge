using QFramework;
using TMPro;
using TPL.PVZR.Classes;
using TPL.PVZR.Classes.DataClasses.Item.Card;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.Helpers.New.DataReader;
using TPL.PVZR.Models;
using UnityEngine;
using UnityEngine.UI;

namespace TPL.PVZR.ViewControllers.Others.UI.ItemView
{
    public class CardViewController : MonoBehaviour, IController
    {
        [SerializeField] private TextMeshProUGUI SunpointText;
        [SerializeField] private Image PlantImage;
        [SerializeField] private Image LockedImage;

        private void UpdateView(CardDefinition cardDefinition, bool locked)
        {
            SunpointText.text = cardDefinition.SunpointCost.ToString();
            PlantImage.sprite = cardDefinition.PlantSprite;
            LockedImage.enabled = locked;
        }

        public void Initialize(CardData cardData)
        {
            Initialize(cardData.CardDefinition, cardData.Locked);
        }

        public void Initialize(CardDefinition cardDefinition, bool locked = false)
        {
            var _GameModel = this.GetModel<IGameModel>();

            UpdateView(cardDefinition, locked);

            _GameModel.GameData.InventoryData.OnPlantBookAdded.Register(bookData =>
                {
                    if (bookData.PlantId == cardDefinition.PlantDef.Id)
                    {
                        UpdateView(PlantConfigReader.GetCardDefinition(new PlantDef(bookData.PlantId, bookData.Variant)),
                            locked);
                    }
                }
            ).UnRegisterWhenGameObjectDestroyed(this);

            _GameModel.GameData.InventoryData.OnPlantBookRemoved.Register(bookData =>
            {
                if (bookData.PlantId == cardDefinition.PlantDef.Id)
                {
                    UpdateView(PlantConfigReader.GetCardDefinition(new PlantDef(bookData.PlantId, PlantVariant.V0)),
                        locked);
                }
            }).UnRegisterWhenGameObjectDestroyed(this);
        }

        public IArchitecture GetArchitecture()
        {
            return PVZRouge.Interface;
        }
    }
}