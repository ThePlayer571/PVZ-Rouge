using QFramework;
using TPL.PVZR.Models;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Managers
{
    public class GameStarter : MonoBehaviour, IController
    {
        private void Start()
        {
            var _ = this.GetModel<IPhaseModel>();
            ActionKit.Sequence()
                .Delay(0.1f)
                .Callback(() =>
                {
                    _.ChangePhase(GamePhase.PreInitialization);
                }).Start(this);
        }

        public IArchitecture GetArchitecture()
        {
            return PVZRouge.Interface;
        }
    }
}