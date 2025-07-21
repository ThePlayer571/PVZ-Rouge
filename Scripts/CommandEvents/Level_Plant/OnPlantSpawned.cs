using TPL.PVZR.ViewControllers.Entities.Plants.Base;
using UnityEngine;

namespace TPL.PVZR.CommandEvents.Level_Gameplay.PlantSpawn
{
    public struct OnPlantSpawned
    {
        public Plant Plant;
        public Vector2Int CellPos;
    }
}