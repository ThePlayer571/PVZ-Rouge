using System;
using QFramework;
using TPL.PVZR.Classes.GameStuff;
using TPL.PVZR.Classes.LevelStuff;
using TPL.PVZR.Core;
using TPL.PVZR.Models;
using UnityEngine;

namespace TPL.PVZR.CommandEvents.__NewlyAdded__
{
    public class SpawnPlantCommand : AbstractCommand
    {
        public SpawnPlantCommand(PlantId id, Vector2Int position, Direction2 direction)
        {
            this._id = id;
            this._position = position;
            this._direction = direction;
        }

        private PlantId _id;
        private Vector2Int _position;
        private Direction2 _direction;

        protected override void OnExecute()
        {
            // 异常处理
            var _LevelGridModel = this.GetModel<ILevelGridModel>();
            if (!_LevelGridModel.IsValidPos(_position)) throw new ArgumentException($"在地图外SpawnPlant, pos:{_position}");
            var targetCell = _LevelGridModel.LevelMatrix[this._position.x, this._position.y];
            if (!_LevelGridModel.CanSpawnPlantOn(_position, _id))
                throw new ArgumentException($"不能在此处生成植物, pos:{_position}");
            //
        }
    }

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

    public struct SpawnPlantEvent
    {
    }

    public struct RemovePlantEvent
    {
    }
}