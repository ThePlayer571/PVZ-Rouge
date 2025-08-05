using QFramework;
using TPL.PVZR.Models;
using TPL.PVZR.Services;

namespace TPL.PVZR.CommandEvents.Level_ChooseSeeds
{
    public class OnStartGameBtnPressedCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            // 异常处理
            var PhaseModel = this.GetModel<IPhaseModel>();
            if (PhaseModel.GamePhase != GamePhase.ChooseSeeds)
                throw new System.Exception($"在不正确的阶段执行OnStartGameBtnPressedCommand：{PhaseModel.GamePhase}");
            
            // 进入游戏阶段
            var phaseService = this.GetService<IPhaseService>();
            phaseService.ChangePhase(GamePhase.ReadyToStart);
        }
    }
}