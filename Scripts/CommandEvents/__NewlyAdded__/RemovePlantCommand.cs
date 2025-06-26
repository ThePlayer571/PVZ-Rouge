using System;
using QFramework;
using TPL.PVZR.Classes.LevelStuff;
using TPL.PVZR.Models;
using UnityEngine;

namespace TPL.PVZR.CommandEvents.__NewlyAdded__
{
    public class RemovePlantCommand : AbstractCommand
    {
        public RemovePlantCommand(Vector2Int position)
        {
            this._position = position;
        }

        private Vector2Int _position;

        protected override void OnExecute()
        {
            // 异常处理
            var _LevelGridModel = this.GetModel<ILevelGridModel>();
            if (!_LevelGridModel.IsValidPos(_position)) throw new ArgumentException($"在地图外RemovePlant，pos:{_position}");
            var targetCell = _LevelGridModel.LevelMatrix[this._position.x, this._position.y];
            if (targetCell.CellPlantState != CellPlantState.HavePlant)
                throw new ArgumentException($"此处不存在植物，却调用了RemovePlantCommand，pos:{_position}");
            //
        }
    }
}