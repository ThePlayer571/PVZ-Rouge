namespace TPL.PVZR.Gameplay.ViewControllers.AutoInteractBase
{
    public abstract class InteractorPointer : AutoInteractor
    {
        protected override void Awake()
        {
            base.Awake();
        }

        private void OnMouseEnter()
        {
            if (_highlightable)
            {
                _highlighted.Value = true;
            }
        }

        private void OnMouseExit()
        {
            _highlighted.Value = false;
        }

        private void OnMouseDown()
        {
            
            if (interactable)
            {
                Interact();
            }
        }
    }
}