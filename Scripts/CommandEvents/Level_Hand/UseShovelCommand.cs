using System;
using QFramework;
using TPL.PVZR.Classes.DataClasses_InLevel;
using TPL.PVZR.Classes.DataClasses;
using TPL.PVZR.Helpers.New.Methods;
using TPL.PVZR.Models;
using TPL.PVZR.Systems;
using TPL.PVZR.Systems.Level_Data;
using UnityEngine;

namespace TPL.PVZR.CommandEvents.Level_Gameplay.HandInputs
{
    public class UseShovelCommand : AbstractCommand
    {
        public UseShovelCommand(Vector2Int position)
        {
            this._position = position;
        }

        private Vector2Int _position;

        protected override void OnExecute()
        {
            var _LevelGridModel = this.GetModel<ILevelGridModel>();
            var _PhaseModel = this.GetModel<IPhaseModel>();
            var _HandSystem = this.GetSystem<IHandSystem>();

            // 异常处理
            if (_PhaseModel.GamePhase != GamePhase.Gameplay)
                throw new Exception($"尝试调用SpawnPlantCommand，但GameState: {_PhaseModel.GamePhase}"); // 游戏阶段正确
            if (HandHelper.IsHandOnUI()) throw new Exception("手不在UI上"); // 手不在UI上
            if (_HandSystem.HandInfo.Value.HandState != HandState.HaveShovel)
                throw new Exception(
                    "尝试调用SpawnPlantCommand，但HandState: {_HandSystem.HandInfo.Value.HandState}"); // 手上持有铲子
            if (!_LevelGridModel.IsValidPos(_position)) throw new ArgumentException($"在地图外ShovelPlant，pos:{_position}");
            var targetCell = _LevelGridModel.LevelMatrix[this._position.x, this._position.y];
            if (targetCell.CellPlantState != CellPlantState.HavePlant)
                throw new ArgumentException($"此处不存在植物，却调用了ShovelPlantCommand，pos:{_position}");
            //
            var plant = targetCell.Plant;
            
            this.SendEvent<OnShovelUsed>();
            plant.Remove();
        }
    }
}