using QFramework;
using TPL.PVZR.Classes;
using TPL.PVZR.CommandEvents.__NewlyAdded__;
using TPL.PVZR.Models;
using TPL.PVZR.Tools;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Entities.Plants
{
    public sealed class Sunflower: Plant
    {
        public override PlantId Id { get; } = PlantId.Sunflower;

        public override void Initialize(Direction2 direction)
        {
            base.Initialize(direction);
            this.HealthPoint = GlobalEntityData.Plant_Default_Health;

            _sunTimer = new Timer(GlobalEntityData.Plant_Sunflower_SpawnSunInterval);
            _sunTimer.SetRemaining(GlobalEntityData.Plant_Sunflower_InitialSpawnSunInterval);
        }

        protected override void Update()
        {
            base.Update();
            _sunTimer.Update(Time.deltaTime);

            if (_PhaseModel.GamePhase != GamePhase.Gameplay) return;
            
            if (_sunTimer.Ready)
            {
                _sunTimer.Reset();
                this.SendCommand<SpawnSunFromPlantCommand>(new SpawnSunFromPlantCommand(this));
            }
        }

        private Timer _sunTimer;
        
    }
}