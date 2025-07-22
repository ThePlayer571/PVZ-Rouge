using QFramework;
using TPL.PVZR.Classes;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.CommandEvents.__NewlyAdded__;
using TPL.PVZR.CommandEvents.Level_Gameplay;
using TPL.PVZR.Models;
using TPL.PVZR.Tools;
using TPL.PVZR.ViewControllers.Entities.Plants.Base;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Entities.Plants
{
    public sealed class Sunflower : Plant
    {
        public override PlantDef Def { get; } = new PlantDef(PlantId.Sunflower, PlantVariant.V0);

        protected override void OnInit()
        {
            this.HealthPoint = GlobalEntityData.Plant_Default_Health;

            _sunTimer = new Timer(GlobalEntityData.Plant_Sunflower_SpawnSunInterval);
            _sunTimer.SetRemaining(GlobalEntityData.Plant_Sunflower_InitialSpawnSunInterval);
        }

        protected override void OnUpdate()
        {
            if (_PhaseModel.GamePhase != GamePhase.Gameplay) return;

            _sunTimer.Update(Time.deltaTime);

            if (_sunTimer.Ready)
            {
                _sunTimer.Reset();
                this.SendCommand<SpawnSunFromPlantCommand>(new SpawnSunFromPlantCommand(this));
            }
        }

        private Timer _sunTimer;
    }
}