using QFramework;
using TPL.PVZR.Architecture;
using TPL.PVZR.Gameplay.ViewControllers.AutoInteractBase;

namespace TPL.PVZR.Gameplay.ViewControllers.InMazeMap
{
    public abstract class Spot:ViewController,IController,IInteractable
    {
        public IArchitecture GetArchitecture()
        {
            return PVZRouge.Interface;
        }

        public abstract void Interact();
    }
}