using QFramework;
using TPL.PVZR.Models;
using TPL.PVZR.Systems;

namespace TPL.PVZR.CommandEvents.Gameplay.HandInputs
{
    public class SelectShovelCommand : AbstractCommand
    {
        public SelectShovelCommand()
        {
        }

        protected override void OnExecute()
        {
            var PhaseModel = this.GetModel<IPhaseModel>();
            var HandSystem = this.GetSystem<IHandSystem>();
            // 异常处理
            if (PhaseModel.GamePhase != GamePhase.Gameplay)
                throw new System.Exception($"在不正确的阶段执行了SelectShovelCommand：{PhaseModel.GamePhase}");
            if (HandSystem.HandInfo.Value.HandState != HandState.Empty)
                throw new System.Exception($"执行了SelectShovelCommand，但是手的状态为：{HandSystem.HandInfo.Value.HandState}");

            //
            this.SendEvent<SelectShovelEvent>();
        }
    }
}