using QFramework;
using TPL.PVZR.Classes.DataClasses_InLevel;
using TPL.PVZR.CommandEvents.Level_Gameplay.HandInputs;
using TPL.PVZR.CommandEvents.Level_Gameplay.PlantSpawn;
using TPL.PVZR.Helpers.New.Methods;
using TPL.PVZR.Models;
using TPL.PVZR.Systems.Level_Data;
using TPL.PVZR.Tools;
using TPL.PVZR.ViewControllers.Entities.Plants.Base;
using UnityEngine;

namespace TPL.PVZR.Services
{
    public interface IHandService : IService
    {
        void SelectSeed(SeedData selectedSeedData);
        void Deselect();
        void SelectShovel();
        void UseShovel();
        void PlantSeedInHand(Direction2 direction, bool bySpawn = true);
    }

    public class HandService : AbstractService, IHandService
    {
        private IHandSystem _HandSystem;
        private ILevelGridModel _LevelGridModel;

        private IPlantService _PlantService;

        protected override void OnInit()
        {
            _HandSystem = this.GetSystem<IHandSystem>();
            _LevelGridModel = this.GetModel<ILevelGridModel>();

            _PlantService = this.GetService<IPlantService>();
        }

        public void SelectSeed(SeedData selectedSeedData)
        {
            _HandSystem.HandInfo.Value = new HandInfo(HandState.HaveSeed, selectedSeedData);
        }

        public void Deselect()
        {
            _HandSystem.HandInfo.Value = new HandInfo(HandState.Empty, null);
        }

        public void SelectShovel()
        {
            _HandSystem.HandInfo.Value = new HandInfo(HandState.HaveShovel, null);
        }

        public void UseShovel()
        {
            var targetCell = _LevelGridModel.GetCell(HandHelper.HandCellPos());
            var plant = targetCell.CellPlantData.GetPlantToShovelFirst();
            _PlantService.RemovePlant(plant);
            this.SendEvent<OnShovelUsed>();
        }

        public void PlantSeedInHand(Direction2 direction, bool bySpawn = true)
        {
            var def = _HandSystem.HandInfo.Value.PickedSeed.CardData.CardDefinition.PlantDef;
            var cellPos = HandHelper.HandCellPos();
            if (bySpawn)
            {
                _PlantService.SpawnPlant(def, cellPos, direction);
            }
            else
            {
                var plant = _LevelGridModel.GetCell(cellPos).CellPlantData.GetPlant(def) as ICanBeStackedOn;
                plant.StackAdd();
            }

            this.SendEvent<OnSeedInHandPlanted>(new OnSeedInHandPlanted
                { Direction = direction, PlantedSeed = _HandSystem.HandInfo.Value.PickedSeed });
        }
    }
}