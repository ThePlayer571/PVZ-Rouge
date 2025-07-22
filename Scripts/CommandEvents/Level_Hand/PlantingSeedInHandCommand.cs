using System;
using System.Linq;
using QFramework;
using TPL.PVZR.CommandEvents.__NewlyAdded__;
using TPL.PVZR.CommandEvents.Level_Gameplay.PlantSpawn;
using TPL.PVZR.Helpers.New.Methods;
using TPL.PVZR.Models;
using TPL.PVZR.Systems;
using TPL.PVZR.Systems.Level_Data;
using TPL.PVZR.Tools;
using TPL.PVZR.ViewControllers.Entities.Plants.Base;

namespace TPL.PVZR.CommandEvents.Level_Gameplay.HandInputs
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
            var def = _HandSystem.HandInfo.Value.PickedSeed.CardData.CardDefinition.PlantDef;
            var canSpawn = _LevelGridModel.CanSpawnPlantOn(cellPos, def);
            var canStack = _LevelGridModel.CanStackPlantOn(cellPos, def);
            if (!canStack && !canSpawn)
                throw new Exception($"无法在此处种植植物，Pos:{cellPos}, Plant: {def}"); // 

            this.SendEvent<OnSeedInHandPlanted>(new OnSeedInHandPlanted
                { Direction = _direction, PlantedSeed = _HandSystem.HandInfo.Value.PickedSeed });

            if (canSpawn)
            {
                this.SendCommand<SpawnPlantCommand>(new SpawnPlantCommand(def, cellPos, _direction));
            }

            else if (canStack)
            {
                var plant =
                    _LevelGridModel.GetCell(cellPos).CellPlantInfo.First(plant => plant.Def == def) as ICanBeStackedOn;
                plant.StackAdd();
            }
        }
    }
}