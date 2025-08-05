using System;
using QFramework;
using TPL.PVZR.Classes.DataClasses.Tomb;
using TPL.PVZR.Models;
using TPL.PVZR.Services;

namespace TPL.PVZR.CommandEvents.__NewlyAdded__
{
    public struct OpenLevelPreviewPanelEvent
    {
        public ITombData Tomb;
        public bool Interactable;
    }

    public class OpenLevelPreviewPanelCommand : AbstractCommand
    {
        public OpenLevelPreviewPanelCommand(ITombData tomb, bool interactable)
        {
            _tomb = tomb;
            _interactable = interactable;
        }

        private ITombData _tomb;
        private bool _interactable;

        protected override void OnExecute()
        {
            var _PhaseModel = this.GetModel<IPhaseModel>();

            // 异常处理
            if (_PhaseModel.GamePhase != GamePhase.MazeMap) throw new Exception();

            //
            this.SendEvent<OpenLevelPreviewPanelEvent>(new OpenLevelPreviewPanelEvent
                { Tomb = _tomb, Interactable = _interactable });
        }
    }
}