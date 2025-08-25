using QFramework;
using TPL.PVZR.Classes.DataClasses_InLevel;
using TPL.PVZR.Models;
using UnityEngine;

namespace TPL.PVZR.Services
{
    public interface ICellTileService : IService
    {
        bool TryPutLadder(Vector2Int cellPos);
        bool TryRemoveLadder(Vector2Int cellPos);
    }

    public class CellTileService : AbstractService, ICellTileService
    {
        private ILevelGridModel _LevelGridModel;

        protected override void OnInit()
        {
            _LevelGridModel = this.GetModel<ILevelGridModel>();
        }

        public bool TryPutLadder(Vector2Int cellPos)
        {
            if (!_LevelGridModel.IsValidPos(cellPos)) return false;

            var cell = _LevelGridModel.GetCell(cellPos);
            if (cell.Is(CellTypeId.Empty))
            {
                _LevelGridModel.SetTile(cellPos.x, cellPos.y, CellTileState.Ladder);
                return true;
            }

            return false;
        }

        public bool TryRemoveLadder(Vector2Int cellPos)
        {
            if (!_LevelGridModel.IsValidPos(cellPos)) return false;

            var cell = _LevelGridModel.GetCell(cellPos);
            if (cell.Is(CellTypeId.Climbable))
            {
                _LevelGridModel.SetTile(cellPos.x, cellPos.y, CellTileState.Empty);
                return true;
            }

            return false;
        }
    }
}