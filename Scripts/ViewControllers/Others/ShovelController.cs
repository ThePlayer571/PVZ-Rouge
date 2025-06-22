using System;
using QFramework;
using TPL.PVZR.Commands.HandCommands;
using TPL.PVZR.Events.HandEvents;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TPL.PVZR.ViewControllers.Others
{
    public class ShovelController : MonoBehaviour, IController, IPointerClickHandler
    {
        [SerializeField] private Image ShovelImage;
        private bool _selected = false;

        private void Awake()
        {
            this.RegisterEvent<SelectShovelEvent>(e =>
            {
                if (_selected) return;
                _selected = true;
                ShovelImage.enabled = false;
            }).UnRegisterWhenGameObjectDestroyed(this);

            this.RegisterEvent<DeselectEvent>(e =>
            {
                if (_selected)
                {
                    _selected = false;
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
                if (_selected)
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
                if (_selected)
                {
                    this.SendCommand<DeselectCommand>();
                }
            }
        }
    }
}