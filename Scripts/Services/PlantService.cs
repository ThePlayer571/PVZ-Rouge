using System.Threading.Tasks;
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
        Task SpawnPlant(PlantDef def, Vector2Int cellPos, Direction2 direction);
        void RemovePlant(Plant plant);
        void ClearCache();
    }

    public class PlantService : AbstractService, IPlantService
    {
        private PlantFactory _plantFactory;

        protected override void OnInit()
        {
            _plantFactory = new PlantFactory();
        }

        public async Task SpawnPlant(PlantDef def, Vector2Int cellPos, Direction2 direction)
        {
            var go = await _plantFactory.SpawnPlant(def, direction, cellPos);
            this.SendEvent<OnPlantSpawned>(new OnPlantSpawned { CellPos = cellPos, Plant = go });
        }

        public void RemovePlant(Plant plant)
        {
            plant.OnRemoved();
            plant.gameObject.DestroySelf();
        }

        public void ClearCache()
        {
            _plantFactory.ClearCache();
        }
    }
}