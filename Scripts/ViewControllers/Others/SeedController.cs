using System;
using QFramework;
using TMPro;
using TPL.PVZR.Classes;
using TPL.PVZR.Classes.GameStuff;
using TPL.PVZR.Classes.LevelStuff;
using TPL.PVZR.Commands.HandCommands;
using TPL.PVZR.Core;
using TPL.PVZR.Events.HandEvents;
using TPL.PVZR.Helpers;
using TPL.PVZR.Models;
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
        private ILevelModel _LevelModel;

        // 数据
        private SeedData _seedData;

        private void Awake()
        {
            _HandSystem = this.GetSystem<IHandSystem>();
            _LevelModel = this.GetModel<ILevelModel>();

            _HandSystem.HandInfo.Register(val =>
            {
                bool selected = (val.HandState == HandState.HaveSeed) && (val.PickedSeed.Index == this._seedData.Index);
                UpdateUI(selected: selected);
            }).UnRegisterWhenGameObjectDestroyed(this);

            _LevelModel.SunPoint.Register(val => { UpdateUI(currentSunpoint: val); })
                .UnRegisterWhenGameObjectDestroyed(this);

            ReferenceHelper.SeedControllers.Add(this);
        }

        public void Initialize(SeedData seedData)
        {
            this._seedData = seedData;
            // UI
            PlantImage.sprite = seedData.CardData.CardDefinition.PlantSprite;
            SunpointCostText.text = seedData.CardData.CardDefinition.SunpointCost.ToString();
            UpdateUI();
        }

        private void OnDestroy()
        {
            ReferenceHelper.SeedControllers.Remove(this);
        }

        private void Update()
        {
            if (!_seedData.ColdTimeTimer.Ready || _seedData.ColdTimeTimer.JustReady) UpdateUI();
        }


        #region UI交互

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (_HandSystem.HandInfo.Value.HandState != HandState.Empty)
                {
                    this.SendCommand<DeselectCommand>();
                }
                else
                {
                    if (!_seedData.ColdTimeTimer.Ready)
                    {
                        NoticeHelper.Notice("冷却中");
                    }
                    else if (_LevelModel.SunPoint.Value < _seedData.CardData.CardDefinition.SunpointCost)
                    {
                        NoticeHelper.Notice("阳光不够");
                    }
                    else
                    {
                        this.SendCommand<SelectSeedCommand>(new SelectSeedCommand(this._seedData));
                    }
                }
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                if (_HandSystem.HandInfo.Value.HandState != HandState.Empty)
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

        #region UI响应

        private void UpdateUI(bool? selected = null, int? currentSunpoint = null)
        {
            selected ??= _HandSystem.HandInfo.Value.HandState == HandState.HaveSeed &&
                         _HandSystem.HandInfo.Value.PickedSeed.Index == this._seedData.Index;
            currentSunpoint ??= _LevelModel.SunPoint.Value;

            // 复杂逻辑：selected == true 时, coldTimeTimer可以为null
            if (selected.Value) // 被选择
            {
                PlantImage.enabled = false;
                GrayMask.enabled = true;
                BlackMask.enabled = true;
                BlackMask.fillAmount = 1;
            }
            else if (_seedData.ColdTimeTimer.Ready) // 冷却完毕
            {
                if (currentSunpoint.Value < _seedData.CardData.CardDefinition.SunpointCost) // 阳光不够
                {
                    PlantImage.enabled = true;
                    GrayMask.enabled = true;
                    BlackMask.enabled = false;
                }
                else // 阳光充足
                {
                    PlantImage.enabled = true;
                    GrayMask.enabled = false;
                    BlackMask.enabled = false;
                }
            }
            else // 冷却中
            {
                PlantImage.enabled = true;
                GrayMask.enabled = true;
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