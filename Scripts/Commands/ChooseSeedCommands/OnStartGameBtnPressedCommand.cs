using QFramework;
using TPL.PVZR.Core;
using TPL.PVZR.Models;

namespace TPL.PVZR.Commands
{
    public class OnStartGameBtnPressedCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            // 异常处理
            var PhaseModel = this.GetModel<IPhaseModel>();
            if (PhaseModel.GamePhase != GamePhase.ChooseSeeds)
                throw new System.Exception($"在不正确的阶段执行OnStartGameBtnPressedCommand：{PhaseModel.GamePhase}");

            // 将选卡数据转录
            
            // 关闭之前的UI界面
            ReferenceHelper.ChooseSeedPanel.HideAllUI();
            
            // 进入游戏阶段
            PhaseModel.ChangePhase(GamePhase.ReadyToStart);
        }
    }
}