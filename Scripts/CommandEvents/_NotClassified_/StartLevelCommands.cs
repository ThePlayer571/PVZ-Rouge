using System.Collections.Generic;
using QFramework;
using TPL.PVZR.Classes.DataClasses.Level;
using TPL.PVZR.Classes.DataClasses.Tomb;
using TPL.PVZR.Models;

namespace TPL.PVZR.CommandEvents._NotClassified_
{
    public class StartLevelCommand : AbstractCommand
    {
        public StartLevelCommand(ITombData tombData)
        {
            _tombData = tombData;
        }

        private ITombData _tombData;

        protected override void OnExecute()

        {
            var _PhaseModel = this.GetModel<IPhaseModel>();
            var _GameModel = this.GetModel<IGameModel>();

            //
            if (_PhaseModel.GamePhase != GamePhase.MazeMap)
                throw new System.Exception($"在不正确的阶段执行StartLevelCommand：{_PhaseModel.GamePhase}");

            //
            _PhaseModel.ChangePhase(GamePhase.LevelPreInitialization,
                new Dictionary<string, object> { { "TombData", _tombData } });
        }
    }
}