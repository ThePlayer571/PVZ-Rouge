using System;
using QFramework;
using TPL.PVZR.Helpers;
using TPL.PVZR.Models;
using TPL.PVZR.ViewControllers;
using TPL.PVZR.ViewControllers.Entities.Plants;
using TPL.PVZR.ViewControllers.Managers;
using UnityEngine;

namespace TPL.PVZR.CommandEvents.__NewlyAdded__
{
    public class SpawnSunFromPlantCommand : AbstractCommand
    {
        public SpawnSunFromPlantCommand(Plant plant)
        {
            this._plant = plant;
        }

        private Plant _plant;

        protected override void OnExecute()
        {
            var _PhaseModel = this.GetModel<IPhaseModel>();
            if (_PhaseModel.GamePhase is not (GamePhase.Gameplay or GamePhase.AllEnemyKilled))
                throw new Exception($"尝试调用SpawnSunFromPlantCommand，但GameState: {_PhaseModel.GamePhase}"); // 游戏阶段正确
            if (_plant == null)
                throw new ArgumentException("尝试调用SpawnSunFromPlantCommand，但Plant对象为null"); // Plant对象不为null

            var go = EntityFactory.SunFactory.SpawnSunWithJump(_plant.transform.position +
                                                               new Vector3(0, TestDataManager.Instance.StartPosOffset,
                                                                   0));
        }
    }

    public class CollectSunCommand : AbstractCommand
    {
        public CollectSunCommand(Sun sun)
        {
            this._sun = sun;
        }

        private Sun _sun;

        protected override void OnExecute()
        {
            var _PhaseModel = this.GetModel<IPhaseModel>();
            var _LevelModel = this.GetModel<ILevelModel>();

            if (_PhaseModel.GamePhase is not (GamePhase.Gameplay or GamePhase.AllEnemyKilled))
                throw new Exception($"尝试调用CollectSunCommand，但GameState: {_PhaseModel.GamePhase}"); // 游戏阶段正确
            if (_sun == null) throw new ArgumentException("尝试调用CollectSunCommand，但Sun对象为null"); // Sun对象不为null

            _LevelModel.SunPoint.Value += _sun.SunPoint;
            _sun.OnCollected();
        }
    }
}