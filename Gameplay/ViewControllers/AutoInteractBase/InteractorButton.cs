using DG.Tweening;
using QFramework;
using TPL.PVZR.Architecture.Events.Input;
using UnityEngine;

namespace TPL.PVZR.Gameplay.ViewControllers.AutoInteractBase
{
    public partial class InteractorButton : AutoInteractor
    {
        protected override void Awake()
        {
            base.Awake();
            this._highlighted.Register(highlighted =>
            {
                if (highlighted)
                {
                    ButtonSprite.DOFade(1, 0.2f);
                }
                else
                {
                    ButtonSprite.DOFade(0, 0.2f);
                }
            }).UnRegisterWhenGameObjectDestroyed(this);
            ButtonSprite.DOFade(0, 0);
            //
            this.RegisterEvent<InputInteractEvent>(@event =>
            {
                if (_highlighted.Value && interactable)
                {
                    Interact();
                }
            }).UnRegisterWhenGameObjectDestroyed(this);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_highlightable)
            {
                _highlighted.Value = true;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            _highlighted.Value = false;
        }
    }
    
}