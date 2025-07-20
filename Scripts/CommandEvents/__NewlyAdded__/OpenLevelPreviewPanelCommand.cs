using System;
using QFramework;
using TPL.PVZR.Classes.DataClasses.Tomb;
using TPL.PVZR.Models;

namespace TPL.PVZR.CommandEvents.__NewlyAdded__
{
    public struct OpenLevelPreviewPanelEvent
    {
        public ITombData Tomb;
    }

    public class OpenLevelPreviewPanelCommand : AbstractCommand
    {
        public OpenLevelPreviewPanelCommand(ITombData tomb)
        {
            _tomb = tomb;
        }

        private ITombData _tomb;

        protected override void OnExecute()
        {
            var _PhaseModel = this.GetModel<IPhaseModel>();

            // 异常处理
            if (_PhaseModel.GamePhase != GamePhase.MazeMap) throw new Exception();

            //
            this.SendEvent<OpenLevelPreviewPanelEvent>(new OpenLevelPreviewPanelEvent { Tomb = _tomb });
        }
    }
}