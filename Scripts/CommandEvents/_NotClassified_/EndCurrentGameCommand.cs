using System;
using QFramework;
using TPL.PVZR.CommandEvents.Phase;
using TPL.PVZR.Models;

namespace TPL.PVZR.CommandEvents._NotClassified_
{
    public class EndCurrentGameCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            var _PhaseModel = this.GetModel<IPhaseModel>();
            // 异常处理
            if (!_PhaseModel.GamePhase.IsInRoughPhase(RoughPhase.Game) ||
                _PhaseModel.GamePhase.IsInRoughPhase(RoughPhase.Loading))
                throw new Exception($"在错的的阶段结束游戏，当前阶段: {_PhaseModel.GamePhase}");

            //
            _PhaseModel.ChangePhase(GamePhase.GameExiting);
        }
    }
}