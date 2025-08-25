using System.Collections.Generic;
using QFramework;
using TPL.PVZR.Models;
using TPL.PVZR.Tools.SoyoFramework;
using UnityEngine;

namespace TPL.PVZR.CommandEvents.Level_Shit
{
    public class ExplodeCommand : AbstractCommand
    {
        public ExplodeCommand(IReadOnlyList<Vector2Int> affectedCells, bool removeLadder)
        {
            _affectedCells = affectedCells;
            _removeLadder = removeLadder;
        }

        private IReadOnlyList<Vector2Int> _affectedCells;
        private bool _removeLadder;

        protected override void OnExecute()
        {
            var _PhaseModel = this.GetModel<IPhaseModel>();
            // 
            if (!_PhaseModel.IsInRoughPhase(RoughPhase.Level))
            {
                $"在错误的阶段调用ExplodeCommand: {_PhaseModel.GamePhase}".LogError();
                return;
            }

            this.SendEvent<OnExploded>(new OnExploded { AffectedCells = _affectedCells, RemoveLadder = _removeLadder });
        }
    }

    public struct OnExploded : IEvent
    {
        public IReadOnlyList<Vector2Int> AffectedCells;
        public bool RemoveLadder;
    }
}