using QFramework;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.Helpers.New.DataReader;
using TPL.PVZR.Helpers.New.Methods;
using TPL.PVZR.Tools;
using TPL.PVZR.ViewControllers.Entities.Plants.Base;
using UnityEngine;

namespace TPL.PVZR.Helpers.New.GameObjectFactory
{
    public class PlantFactory
    {
        public Plant SpawnPlant(PlantDef def, Direction2 direction, Vector2Int cellPos)
        {
            var plantPrefab = PlantConfigReader.GetPlantPrefab(def);

            var plant = plantPrefab
                .Instantiate(LevelGridHelper.CellToWorldBottom(cellPos), Quaternion.identity)
                .GetComponent<Plant>();
            plant.Initialize(direction);

            return plant;
        }
    }
}