using System;
using QFramework;
using TPL.PVZR.Helpers;
using TPL.PVZR.Models;
using TPL.PVZR.ViewControllers.Entities.Plants.Base;
using UnityEngine;

namespace TPL.PVZR.CommandEvents.__NewlyAdded__
{
    public class SpawnCoinFromPlantCommand : AbstractCommand
    {
        public SpawnCoinFromPlantCommand(Plant plant, CoinId coinId)
        {
            _plant = plant;
            _coinId = coinId;
        }

        private Plant _plant;
        private CoinId _coinId;

        protected override void OnExecute()
        {
            var _PhaseModel = this.GetModel<IPhaseModel>();
            if (_PhaseModel.GamePhase is not (GamePhase.Gameplay or GamePhase.AllEnemyKilled))
                throw new Exception($"尝试调用SpawnCoinFromPlantCommand，但GameState: {_PhaseModel.GamePhase}"); // 游戏阶段正确
            if (_plant == null)
                throw new ArgumentException("尝试调用SpawnCoinFromPlantCommand，但Plant对象为null"); // Plant对象不为null

            EntityFactory.CoinFactory.SpawnCoinWithJump(_coinId, _plant.transform.position);
        }
    }
}