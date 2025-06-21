using QFramework;
using TPL.PVZR.Events.HandEvents;
using TPL.PVZR.Models;
using TPL.PVZR.Systems;
using TPL.PVZR.ViewControllers.Others;

namespace TPL.PVZR.Commands.HandCommands
{
    public class SelectSeedCommand : AbstractCommand
    {
        public SelectSeedCommand(SeedController selectedSeed)
        {
            _selectedSeed = selectedSeed;
        }

        private SeedController _selectedSeed;

        protected override void OnExecute()
        {
            var PhaseModel = this.GetModel<IPhaseModel>();
            var HandSystem = this.GetSystem<IHandSystem>();
            // 异常处理
            if (PhaseModel.GamePhase != GamePhase.Gameplay)
                throw new System.Exception($"在不正确的阶段执行了SelectSeedCommand：{PhaseModel.GamePhase}");
            if (HandSystem.HandState != HandState.Empty)
                throw new System.Exception($"执行了SelectSeedCommand，但是手的状态为：{HandSystem.HandState}");

            // 
            this.SendEvent<SelectSeedEvent>(new SelectSeedEvent { SelectedSeed = _selectedSeed });
        }
    }
}