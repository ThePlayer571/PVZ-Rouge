using QFramework;
using TPL.PVZR.Models;
using TPL.PVZR.Services;
using TPL.PVZR.Systems;
using TPL.PVZR.Systems.Level_Data;
using TPL.PVZR.Tools.SoyoFramework;

namespace TPL.PVZR.CommandEvents.Level_Gameplay.HandInputs
{
    public class DeselectCommand : AbstractCommand
    {
        public DeselectCommand()
        {
        }

        protected override void OnExecute()
        {
            var PhaseModel = this.GetModel<IPhaseModel>();
            var HandSystem = this.GetSystem<IHandSystem>();
            // 异常处理
            if (PhaseModel.GamePhase != GamePhase.Gameplay)
                throw new System.Exception($"在不正确的阶段执行了DeselectCommand：{PhaseModel.GamePhase}");
            if (HandSystem.HandInfo.Value.HandState == HandState.Empty)
                throw new System.Exception($"执行了DeselectCommand，但是手的状态为：{HandSystem.HandInfo.Value.HandState}");

            //
            var handService = this.GetService<IHandService>();
            handService.Deselect();
        }
    }
}