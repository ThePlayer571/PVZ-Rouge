using QFramework;
using TPL.PVZR.Architecture;
using UnityEngine;

namespace TPL.PVZR.Gameplay.ViewControllers.InMazeMap.PopInfo
{
    public abstract class UISpotInfo : MonoBehaviour, IController
    {
        public abstract void Show();
        public abstract void HideInstant();
        public abstract void HideAndDestroy();
        public IArchitecture GetArchitecture()
        {
            return PVZRouge.Interface;
        }
    }
}