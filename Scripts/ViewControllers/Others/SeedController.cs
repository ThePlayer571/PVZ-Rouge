using System;
using QFramework;
using TMPro;
using TPL.PVZR.Classes;
using TPL.PVZR.Classes.GameStuff;
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

        private void Awake()
        {
            this._HandSystem = this.GetSystem<IHandSystem>();

            this.RegisterEvent<SelectSeedEvent>(e =>
            {
                if (ReferenceEquals(e.SelectedSeed, this))
                {
                    isSelected = true;
                    UpdateUI();
                }
            });

            this.RegisterEvent<DeselectEvent>(e =>
            {
                if (isSelected)
                {
                    isSelected = false;
                    UpdateUI();
                }
            });
            
            ReferenceHelper.SeedControllers.Add(this);
        }

        private void OnDestroy()
        {
            ReferenceHelper.SeedControllers.Remove(this);
        }

        // 数据
        public CardData CardData { get; private set; }
        public int Index { get; private set; }

        #region Logic

        // 变量
        public bool isSelected { get; private set; }
        [SerializeField] private Timer coldTimeTimer;

        private void Update()
        {
            coldTimeTimer.Update(Time.deltaTime);

            if (!coldTimeTimer.Ready || coldTimeTimer.JustReady) UpdateUI();
        }

// 初始化
        public void Initialize(CardData cardData, int index)
        {
            // 数据
            this.CardData = cardData;
            this.Index = index;
            // UI
            PlantImage.sprite = cardData.CardDefinition.PlantSprite;
            SunpointCostText.text = cardData.CardDefinition.SunpointCost.ToString();
            // Logic
            coldTimeTimer = new Timer(CardData.CardDefinition.ColdTime);
        }

// 交互
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
                    this.SendCommand<SelectSeedCommand>(new SelectSeedCommand(this));
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

        #region UI

        private void UpdateUI()
        {
            if (isSelected) // 被选择
            {
                PlantImage.enabled = false;
                GrayMask.enabled = false;
                BlackMask.enabled = true;
                BlackMask.fillAmount = 1;
            }
            else if (coldTimeTimer.Ready) // 冷却完毕
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
                var val = coldTimeTimer.Remaining / coldTimeTimer.Duration;
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