using System;
using QFramework;
using TPL.PVZR.Models;

namespace TPL.PVZR.Commands
{
    public class PressTempStartGameButtonCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            var phaseModel = this.GetModel<IPhaseModel>();
            if (phaseModel.GamePhase != GamePhase.MainMenu)
                throw new Exception($"在不合适的状态下执行了GameInitCommand: {phaseModel.GamePhase}");
            // 进入一局游戏
            
        }
    }
}