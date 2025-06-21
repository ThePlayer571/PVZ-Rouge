using QFramework;
using TPL.PVZR.Classes.GameStuff;
using TPL.PVZR.Core;
using TPL.PVZR.Helpers;
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