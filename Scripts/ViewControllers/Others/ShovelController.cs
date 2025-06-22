using System;
using QFramework;
using TPL.PVZR.Commands.HandCommands;
using TPL.PVZR.Events.HandEvents;
using TPL.PVZR.Systems;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TPL.PVZR.ViewControllers.Others
{
    public class ShovelController : MonoBehaviour, IController, IPointerClickHandler
    {
        [SerializeField] private Image ShovelImage;
        private IHandSystem _HandSystem;

        private void Awake()
        {
            _HandSystem = this.GetSystem<IHandSystem>();

            _HandSystem.HandInfo.RegisterWithInitValue(val =>
            {
                if (val.HandState == HandState.HaveShovel)
                {
                    ShovelImage.enabled = false;
                }
                else
                {
                    ShovelImage.enabled = true;
                }
            }).UnRegisterWhenGameObjectDestroyed(this);
        }


        public IArchitecture GetArchitecture()
        {
            return PVZRouge.Interface;
        }

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
                    this.SendCommand<SelectShovelCommand>();
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
    }
}