using System;
using QFramework;
using TPL.PVZR.Classes.GameStuff;
using TPL.PVZR.Classes.LevelStuff;
using TPL.PVZR.Core;
using TPL.PVZR.Helpers;
using TPL.PVZR.Helpers.Factory;
using TPL.PVZR.Models;
using UnityEngine;

namespace TPL.PVZR.CommandEvents.__NewlyAdded__
{
    public class SpawnPlantCommand : AbstractCommand
    {
        public SpawnPlantCommand(PlantId id, Vector2Int cellPos, Direction2 direction)
        {
            this._id = id;
            this._cellPos = cellPos;
            this._direction = direction;
        }

        private PlantId _id;
        private Vector2Int _cellPos;
        private Direction2 _direction;

        protected override void OnExecute()
        {
            var _LevelGridModel = this.GetModel<ILevelGridModel>();
            var _PhaseModel = this.GetModel<IPhaseModel>();

            // 异常处理
            if (_PhaseModel.GamePhase != GamePhase.Gameplay)
                throw new Exception($"尝试调用SpawnPlantCommand，但GameState: {_PhaseModel.GamePhase}"); // 游戏阶段正确
            if (!_LevelGridModel.CanSpawnPlantOn(_cellPos, _id))
                throw new Exception($"无法在此处种植植物，Pos:{_cellPos}, Plant: {_id}"); // 

            //
            EntityFactory.PlantFactory.SpawnPlant(_id, _direction, _cellPos);
            var targetCell = _LevelGridModel.GetCell(_cellPos);
            targetCell.CellPlantState = CellPlantState.HavePlant;
        }
    }
}