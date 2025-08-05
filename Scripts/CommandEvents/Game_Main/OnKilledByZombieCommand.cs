using QFramework;
using TPL.PVZR.Models;
using TPL.PVZR.Services;
using TPL.PVZR.Tools.SoyoFramework;

namespace TPL.PVZR.CommandEvents._NotClassified_
{
    public class OnKilledByZombieCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            var _PhaseModel = this.GetModel<IPhaseModel>();

            if (_PhaseModel.GamePhase is not (GamePhase.ChooseSeeds or GamePhase.Gameplay or GamePhase.AllEnemyKilled))
                throw new System.Exception($"在不正确的阶段执行OnKilledByZombieCommand：{_PhaseModel.GamePhase}");

            //
            var gamePhaseChangeService = this.GetService<IGamePhaseChangeService>();
            gamePhaseChangeService.EnterLevelDefeatPanel();
        }
    }
}