using QFramework;
using TPL.PVZR.Classes;
using TPL.PVZR.CommandEvents.__NewlyAdded__;
using TPL.PVZR.Models;
using TPL.PVZR.Tools;
using TPL.PVZR.Tools.Random;
using TPL.PVZR.ViewControllers.Entities.Plants.Base;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Entities.Plants
{
    public sealed class Marigold : Plant
    {
        public override PlantId Id { get; } = PlantId.Marigold;

        protected override void OnInit()
        {
            HealthPoint = GlobalEntityData.Plant_Default_Health;

            _coinTimer = new Timer(GlobalEntityData.Plant_Marigold_SpawnCoinInterval);
            _coinTimer.SetRemaining(GlobalEntityData.Plant_Marigold_InitialSpawnCoinInterval);
        }

        protected override void Update()
        {
            base.Update();
            _coinTimer.Update(Time.deltaTime);

            if (_PhaseModel.GamePhase != GamePhase.Gameplay ||
                _LevelModel.CurrentWave.Value == _LevelModel.LevelData.TotalWaveCount) return;

            if (_coinTimer.Ready)
            {
                _coinTimer.Reset();
                var coinId = RandomHelper.Default.Value > 0.7f ? CoinId.Gold : CoinId.Silver;
                this.SendCommand<SpawnCoinFromPlantCommand>(new SpawnCoinFromPlantCommand(this, coinId));
            }
        }


        private Timer _coinTimer;
    }
}