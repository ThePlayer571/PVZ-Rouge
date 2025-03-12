using System;
using UnityEngine;
using QFramework;
using UnityEngine.UI;

namespace TPL.PVZR
{
    public class Card : ViewController, IController
    {
        private Button Btn;
        private LevelSystem _LevelSystem;
        private void Awake()
        {
            _LevelSystem = this.GetSystem<LevelSystem>();
            Btn = GetComponent<Button>();
            Btn.LogInfo();
            Btn.onClick.AddListener(OnClick);
        }

        private bool isSelected = false;

        private void OnClick()
        {
            if (!isSelected) // 在Inventory里
            {
                if (_LevelSystem.canAddCard)
                {
                    transform.SetParent(transform.parent.parent.Find("ChosenCards"));
                    isSelected = true;
                    _LevelSystem.AddCard(this);
                }
            }
            else // 在Chosen里
            {
                transform.SetParent(transform.parent.parent.Find("Inventory"));
                isSelected = false;
                _LevelSystem.RemoveCard(this);
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
    }
}