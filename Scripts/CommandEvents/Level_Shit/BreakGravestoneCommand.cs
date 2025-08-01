using QFramework;
using TPL.PVZR.Classes.DataClasses_InLevel;
using TPL.PVZR.Models;
using UnityEngine;

namespace TPL.PVZR.CommandEvents.Level_Shit
{
    public interface IServiceCommand : ICommand
    {
    }

    public struct OnGravestoneBroken
    {
        public Vector2Int CellPos;
    }

    public class BreakGravestoneCommand : AbstractCommand, IServiceCommand
    {
        public BreakGravestoneCommand(Vector2Int cellPos)
        {
            _cellPos = cellPos;
        }

        private Vector2Int _cellPos;

        protected override void OnExecute()
        {
            var _LevelGridModel = this.GetModel<ILevelGridModel>();
            _LevelGridModel.SetTile(_cellPos.x, _cellPos.y, CellTileState.Empty);
            this.SendEvent<OnGravestoneBroken>(new OnGravestoneBroken { CellPos = _cellPos });
        }
    }
}