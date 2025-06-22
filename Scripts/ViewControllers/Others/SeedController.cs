using System;
using QFramework;
using TMPro;
using TPL.PVZR.Classes;
using TPL.PVZR.Classes.GameStuff;
using TPL.PVZR.Classes.LevelStuff;
using TPL.PVZR.Commands.HandCommands;
using TPL.PVZR.Core;
using TPL.PVZR.Events.HandEvents;
using TPL.PVZR.Systems;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TPL.PVZR.ViewControllers.Others
{
    public class SeedController : MonoBehaviour, IController, IPointerClickHandler, IPointerEnterHandler,
        IPointerExitHandler
    {
        // 引用
        [SerializeField] private Image PlantImage;
        [SerializeField] private TextMeshProUGUI SunpointCostText;
        [SerializeField] private Image GrayMask;
        [SerializeField] private Image BlackMask;

        private IHandSystem _HandSystem;

        // 数据
        private SeedData _seedData;

        // 变量
        private bool _selected = false;

        private void Awake()
        {
            this._HandSystem = this.GetSystem<IHandSystem>();

            this.RegisterEvent<SelectSeedEvent>(e =>
            {
                if (e.SelectedSeedData.Index == this._seedData.Index)
                {
                    this._selected = true;
                    UpdateUI();
                }
            });

            this.RegisterEvent<DeselectEvent>(e =>
            {
                if (this._selected)
                {
                    this._selected = false;
                    UpdateUI();
                }
            });

            ReferenceHelper.SeedControllers.Add(this);
        }

        private void OnDestroy()
        {
            ReferenceHelper.SeedControllers.Remove(this);
        }


        private void Update()
        {
            if (!_seedData.ColdTimeTimer.Ready || _seedData.ColdTimeTimer.JustReady) UpdateUI();
        }

        public void Initialize(SeedData seedData)
        {
            this._seedData = seedData;
            // UI
            PlantImage.sprite = seedData.CardData.CardDefinition.PlantSprite;
            SunpointCostText.text = seedData.CardData.CardDefinition.SunpointCost.ToString();
        }

        #region UI交互

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (_HandSystem.HandState != HandState.Empty)
                {
                    this.SendCommand<DeselectCommand>();
                }
                else
                {
                    this.SendCommand<SelectSeedCommand>(new SelectSeedCommand(this._seedData));
                }
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                if (_HandSystem.HandState != HandState.Empty)
                {
                    this.SendCommand<DeselectCommand>();
                }
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
        }

        public void OnPointerExit(PointerEventData eventData)
        {
        }

        #endregion

        #region UI表现

        private void UpdateUI()
        {
            if (_selected) // 被选择
            {
                PlantImage.enabled = false;
                GrayMask.enabled = false;
                BlackMask.enabled = true;
                BlackMask.fillAmount = 1;
            }
            else if (_seedData.ColdTimeTimer.Ready) // 冷却完毕
            {
                PlantImage.enabled = true;
                GrayMask.enabled = false;
                BlackMask.enabled = false;
            }
            else // 冷却中
            {
                PlantImage.enabled = true;
                GrayMask.enabled = false;
                BlackMask.enabled = true;
                var val = _seedData.ColdTimeTimer.Remaining / _seedData.ColdTimeTimer.Duration;
                BlackMask.fillAmount = val;
            }
        }

        #endregion


        public IArchitecture GetArchitecture()
        {
            return PVZRouge.Interface;
        }
    }
}