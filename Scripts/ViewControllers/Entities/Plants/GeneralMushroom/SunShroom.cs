using QFramework;
using TPL.PVZR.Classes;
using TPL.PVZR.Classes.DataClasses.Level;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.CommandEvents.__NewlyAdded__;
using TPL.PVZR.CommandEvents.Level_Gameplay;
using TPL.PVZR.Helpers.New.GameObjectFactory;
using TPL.PVZR.Models;
using TPL.PVZR.Tools;
using TPL.PVZR.ViewControllers.Entities.Plants.Base;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Entities.Plants
{
    public sealed class SunShroom : SleepyMushroomBase
    {
        public override PlantDef Def { get; } = new PlantDef(PlantId.SunShroom, PlantVariant.V0);

        protected override void OnShroomInit()
        {
            this.HealthPoint = GlobalEntityData.Plant_Default_Health;

            _sunTimer = new Timer(GlobalEntityData.Plant_Sunflower_SpawnSunInterval);
            _sunTimer.SetRemaining(GlobalEntityData.Plant_Sunflower_InitialSpawnSunInterval);

            _growTimer = new Timer(GlobalEntityData.Plant_SunShroom_GrowTime);
        }

        protected override void OnShroomUpdate()
        {
            // 长大
            if (!_grown)
            {
                _growTimer.Update(Time.deltaTime);
                if (_growTimer.Ready)
                {
                    _grown = true;
                }
            }


            //
            if (_PhaseModel.GamePhase != GamePhase.Gameplay) return;


            // 产阳光
            _sunTimer.Update(Time.deltaTime);
            if (_sunTimer.Ready)
            {
                _sunTimer.Reset();
                var sunId = _grown ? SunId.Sun : SunId.SmallSun;
                this.SendCommand<SpawnSunFromPlantCommand>(new SpawnSunFromPlantCommand(this, sunId));
            }
        }

        private Timer _sunTimer;
        private Timer _growTimer;
        private bool _grown;
    }
}