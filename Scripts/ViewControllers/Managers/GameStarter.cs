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
            var _ = this.GetModel<IPhaseModel>();
            ActionKit.Sequence()
                .Delay(0.1f)
                .Callback(() =>
                {
                    var phaseService = this.GetService<IPhaseService>();
                    phaseService.ChangePhase(GamePhase.PreInitialization);
                }).Start(this);

            // ActionKit.Sequence()
            //     .Delay(0.5f)
            //     .Callback(() =>
            //     {
            //         PlantHelper.CreatePlant(PlantId.PeaShooter, Direction2.Right);
            //     }).Start(this);
        }

        public IArchitecture GetArchitecture()
        {
            return PVZRouge.Interface;
        }
    }
}