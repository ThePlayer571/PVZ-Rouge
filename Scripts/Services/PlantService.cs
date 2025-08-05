using QFramework;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.CommandEvents.Level_Gameplay.PlantSpawn;
using TPL.PVZR.Helpers.New.GameObjectFactory;
using TPL.PVZR.Tools;
using TPL.PVZR.ViewControllers.Entities.Plants.Base;
using UnityEngine;

namespace TPL.PVZR.Services
{
    public interface IPlantService : IService
    {
        void SpawnPlant(PlantDef def, Vector2Int cellPos, Direction2 direction);
        void RemovePlant(Plant plant);
    }

    public class PlantService : AbstractService, IPlantService
    {
        private PlantFactory _plantFactory;

        protected override void OnInit()
        {
            _plantFactory = new PlantFactory();
        }

        public void SpawnPlant(PlantDef def, Vector2Int cellPos, Direction2 direction)
        {
            var go = _plantFactory.SpawnPlant(def, direction, cellPos);
            this.SendEvent<OnPlantSpawned>(new OnPlantSpawned { CellPos = cellPos, Plant = go });
        }

        public void RemovePlant(Plant plant)
        {
            plant.OnRemoved();
            plant.gameObject.DestroySelf();
        }
    }
}