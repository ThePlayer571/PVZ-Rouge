using QFramework;
using TPL.PVZR.Architecture;
using TPL.PVZR.Architecture.Systems.InLevel;
using TPL.PVZR.Architecture.Systems.PhaseSystems;
using TPL.PVZR.Gameplay.Class.Items;
using UnityEngine.UI;

namespace TPL.PVZR.Gameplay.ViewControllers.UI
{
    public partial class UICard : ViewController, IController
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

        public Card card { get;private set; }

        public void Init(Card card)
        {
            this.card = card;
            Plant.sprite = this.card.cardSO.seedSO.plantSprite;
            SunText.text = this.card.cardSO.seedSO.sunpointCost.ToString();
        }
        
    }
}