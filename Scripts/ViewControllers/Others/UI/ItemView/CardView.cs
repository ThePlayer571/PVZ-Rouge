using TMPro;
using TPL.PVZR.Classes.DataClasses.Item.Card;
using UnityEngine;
using UnityEngine.UI;

namespace TPL.PVZR.ViewControllers.Others.UI.ItemView
{
    public class CardView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI SunpointText;
        [SerializeField] private Image PlantImage;
        [SerializeField] private Image LockedImage;


        public void Initialize(CardData cardData)
        {
            SunpointText.text = cardData.CardDefinition.SunpointCost.ToString();
            PlantImage.sprite = cardData.CardDefinition.PlantSprite;
            LockedImage.enabled = cardData.Locked;
        }

        public void Initialize(CardDefinition cardDefinition, bool locked = false)
        {
            SunpointText.text = cardDefinition.SunpointCost.ToString();
            PlantImage.sprite = cardDefinition.PlantSprite;
            LockedImage.enabled = locked;
        }
    }
}