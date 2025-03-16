using System;
using UnityEngine;
using QFramework;
using UnityEngine.UI;

namespace TPL.PVZR
{
    public partial class Card : ViewController, IController
    {
        private Button Btn;
        //
        private ILevelSystem _LevelSystem;
        private IChooseCardSystem _ChooseCardSystem;
        private void Awake()
        {
            _LevelSystem = this.GetSystem<ILevelSystem>();
            _ChooseCardSystem = this.GetSystem<IChooseCardSystem>();
            Btn = GetComponent<Button>();
            Btn.onClick.AddListener(OnClick);
        }

        private bool isSelected = false;

        private void OnClick()
        {
            if (!isSelected) // 在Inventory里
            {
                if (_ChooseCardSystem.canAddCard)
                {
                    transform.SetParent(transform.parent.parent.Find("ChosenCards"));
                    isSelected = true;
                    _ChooseCardSystem.AddCard(this);
                }
            }
            else // 在Chosen里
            {
                transform.SetParent(transform.parent.parent.Find("Inventory"));
                isSelected = false;
                _ChooseCardSystem.RemoveCard(this);
            }
        }

        public IArchitecture GetArchitecture()
        {
           return PVZRouge.Interface;
        }

        private void OnDestroy()
        {
            Btn.onClick.RemoveListener(OnClick);
        }
        
        
        // 数据
        
        public CardDataSO cardData;
        public void Init(CardDataSO cardDataSO)
        {
            cardData = cardDataSO;
            Plant.sprite = cardData.seedData.plantSprite;
            SunText.text = cardData.seedData.sunpointCost.ToString();
        }
        
    }
}