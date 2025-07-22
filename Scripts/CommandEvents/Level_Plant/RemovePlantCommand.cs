using System;
using QFramework;
using TPL.PVZR.Classes;
using TPL.PVZR.Classes.DataClasses_InLevel;
using TPL.PVZR.Classes.DataClasses;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.Helpers;
using TPL.PVZR.Helpers.New.GameObjectFactory;
using TPL.PVZR.Models;
using TPL.PVZR.Tools;
using TPL.PVZR.Tools.SoyoFramework;
using TPL.PVZR.ViewControllers.Entities.Plants.Base;
using UnityEngine;

namespace TPL.PVZR.CommandEvents.Level_Gameplay.PlantSpawn
{
    public struct RemovePlantEvent : IServiceEvent
    {
        public Plant Plant;
    }

    public class RemovePlantCommand : AbstractCommand
    {
        public RemovePlantCommand(Plant plant)
        {
            this._plant = plant;
        }

        private Plant _plant;

        protected override void OnExecute()
        {
            var _PhaseModel = this.GetModel<IPhaseModel>();

            // 异常处理
            if (_PhaseModel.GamePhase != GamePhase.Gameplay)
                throw new Exception($"尝试调用RemovePlantCommand，但GameState: {_PhaseModel.GamePhase}"); // 游戏阶段正确

            //
            this.SendEvent<RemovePlantEvent>(new RemovePlantEvent { Plant = _plant });
        }
    }
}