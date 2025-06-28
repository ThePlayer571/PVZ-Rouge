using System;
using QFramework;
using TPL.PVZR.Core;
using TPL.PVZR.Events.HandEvents;
using TPL.PVZR.Helpers.Methods;
using TPL.PVZR.Models;
using TPL.PVZR.Systems;
using UnityEngine;

namespace TPL.PVZR.CommandEvents.__NewlyAdded__
{
    public class PlantingSeedInHandCommand : AbstractCommand
    {
        public PlantingSeedInHandCommand(Direction2 direction)
        {
            this._direction = direction;
        }

        private Direction2 _direction;

        protected override void OnExecute()
        {
            var _LevelGridModel = this.GetModel<ILevelGridModel>();
            var _PhaseModel = this.GetModel<IPhaseModel>();
            var _HandSystem = this.GetSystem<IHandSystem>();

            // 异常处理
            if (_PhaseModel.GamePhase != GamePhase.Gameplay)
                throw new Exception($"尝试调用SpawnPlantCommand，但GameState: {_PhaseModel.GamePhase}"); // 游戏阶段正确
            if (HandHelper.IsHandOnUI()) throw new Exception("手不在UI上"); // 手不在UI上
            if (_HandSystem.HandInfo.Value.HandState != HandState.HaveSeed)
                throw new Exception(
                    "尝试调用SpawnPlantCommand，但HandState: {_HandSystem.HandInfo.Value.HandState}"); // 手上持有植物
            var cellPos = HandHelper.HandCellPos();
            if (!HandHelper.DaveCanReach(cellPos))
                throw new Exception($"尝试调用SpawnPlantCommand，但是手够不到植物种植处"); // 手能够到目标位置
            var id = _HandSystem.HandInfo.Value.PickedSeed.CardData.CardDefinition.Id;
            if (!_LevelGridModel.CanSpawnPlantOn(cellPos, id))
                throw new Exception($"无法在此处种植植物，Pos:{cellPos}, Plant: {id}"); // 

            this.SendEvent<PlantingSeedInHandEvent>(new PlantingSeedInHandEvent
                { Direction = _direction, PlantedSeed = _HandSystem.HandInfo.Value.PickedSeed });
            this.SendCommand<SpawnPlantCommand>(new SpawnPlantCommand(id, cellPos, _direction));
        }
    }
}