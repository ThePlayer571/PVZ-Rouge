using QFramework;
using TPL.PVZR.Models;
using TPL.PVZR.Services;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Managers
{
    public class GameStarter : MonoBehaviour, IController
    {
        private void Start()
        {
            Debug.Log("GameStarter Start");
            var _ = this.GetModel<IPhaseModel>();
            ActionKit.Sequence()
                .Delay(0.1f)
                .Callback(() =>
                {
                    var phaseService = this.GetService<IPhaseService>();
                    phaseService.ChangePhase(GamePhase.PreInitialization);
                }).Start(this);
        }

        public IArchitecture GetArchitecture()
        {
            return PVZRouge.Interface;
        }
    }
}