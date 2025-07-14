using TMPro;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Others.UI.ItemView
{
    public class CoinViewController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI coinText;

        public void Initialize(int amount)
        {
            coinText.text = amount.ToString();
        }
    }
}