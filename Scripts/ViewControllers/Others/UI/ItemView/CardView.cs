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


        public void Initialize(CardData cardData)
        {
            SunpointText.text = cardData.CardDefinition.SunpointCost.ToString();
            PlantImage.sprite = cardData.CardDefinition.PlantSprite;
        }
    }
}