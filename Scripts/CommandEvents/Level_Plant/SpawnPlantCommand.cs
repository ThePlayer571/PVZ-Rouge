using System;
using QFramework;
using TPL.PVZR.Classes;
using TPL.PVZR.Classes.DataClasses_InLevel;
using TPL.PVZR.Classes.DataClasses;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.Helpers;
using TPL.PVZR.Helpers.New.GameObjectFactory;
using TPL.PVZR.Models;
using TPL.PVZR.Services;
using TPL.PVZR.Tools;
using TPL.PVZR.Tools.SoyoFramework;
using UnityEngine;

namespace TPL.PVZR.CommandEvents.Level_Gameplay.PlantSpawn
{
    public class SpawnPlantCommand : AbstractCommand
    {
        public SpawnPlantCommand(PlantDef def, Vector2Int cellPos, Direction2 direction)
        {
            this._def = def;
            this._cellPos = cellPos;
            this._direction = direction;
        }

        private PlantDef _def;
        private Vector2Int _cellPos;
        private Direction2 _direction;

        protected override void OnExecute()
        {
            var _LevelGridModel = this.GetModel<ILevelGridModel>();
            var _PhaseModel = this.GetModel<IPhaseModel>();

            // 异常处理
            if (_PhaseModel.GamePhase != GamePhase.Gameplay)
                throw new Exception($"尝试调用SpawnPlantCommand，但GameState: {_PhaseModel.GamePhase}"); // 游戏阶段正确
            if (!_LevelGridModel.CanSpawnPlantOn(_cellPos, _def))
                throw new Exception($"无法在此处种植植物，Pos:{_cellPos}, Plant: {_def}"); // 

            //
            var plantService = this.GetService<IPlantService>();
            plantService.SpawnPlant(_def, _cellPos, _direction);
        }
    }
}