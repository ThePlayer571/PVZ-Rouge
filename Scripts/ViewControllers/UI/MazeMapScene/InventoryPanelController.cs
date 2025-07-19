using System;
using System.Collections.Generic;
using QFramework;
using TMPro;
using TPL.PVZR.Classes.DataClasses.Item.Card;
using TPL.PVZR.Helpers;
using TPL.PVZR.Helpers.New.GameObjectFactory;
using TPL.PVZR.Models;
using UnityEngine;
using UnityEngine.UI;

namespace TPL.PVZR.ViewControllers.Others.UI.MazeMap
{
    public class InventoryPanelController : MonoBehaviour, IController
    {
        [SerializeField] private Toggle toggle;
        [SerializeField] private RectTransform View;
        [SerializeField] private TextMeshProUGUI SeedSlotCountText;

        private IGameModel _GameModel;
        
        private void Start()
        {
            _GameModel = this.GetModel<IGameModel>();
            
            toggle.onValueChanged.AddListener(Display);

            _GameModel.GameData.InventoryData.SeedSlotCount.RegisterWithInitValue(val =>
                {
                    SeedSlotCountText.text = "SeedSlotCount: " + val.ToString();
                }).UnRegisterWhenGameObjectDestroyed(this);

        }

        private void Display(bool show)
        {
            if (show) View.Show();
            else View.Hide();
        }

        public IArchitecture GetArchitecture()
        {
            return PVZRouge.Interface;
        }
    }
}