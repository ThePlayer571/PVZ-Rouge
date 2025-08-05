using QFramework;
using TPL.PVZR.Models;
using TPL.PVZR.Services;

namespace TPL.PVZR.CommandEvents._NotClassified_
{
    public class OnCollectLevelEndObjectCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            var _PhaseModel = this.GetModel<IPhaseModel>();

            // 异常处理
            if (_PhaseModel.GamePhase != GamePhase.AllEnemyKilled)
                throw new System.Exception($"在不正确的阶段执行OnCollectLevelEndObjectCommand：{_PhaseModel.GamePhase}");
            
            //
            var gamePhaseChangeService = this.GetService<IGamePhaseChangeService>();
            gamePhaseChangeService.EnterMazeMapWithLevelPassed();
        }
    }
}