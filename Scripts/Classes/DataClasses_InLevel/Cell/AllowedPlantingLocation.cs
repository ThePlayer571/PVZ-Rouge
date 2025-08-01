using System;
using System.Collections.Generic;
using System.Linq;
using QFramework;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.Helpers.New.DataReader;
using TPL.PVZR.ViewControllers.Entities.Plants;
using TPL.PVZR.ViewControllers.Entities.Plants.Base;

namespace TPL.PVZR.Classes.DataClasses_InLevel
{
    /// <summary>
    /// 以植物为第一人称，描述“我的种植位置应该满足的条件”
    /// </summary>
    public class PlantingLocationCondition
    {
        // spawn
        public bool bannedSpawn = false;

        // OR Condition
        public bool canAndOnlyCanSpawnOnSleepingShroom = false;

        // AND Condition
        public List<CellTypeId> currentCellConditions = new();
        public List<CellTypeId> belowCellConditions = new();
        public List<CellTypeId> aboveCellConditions = new();

        // stack
        public bool canStackOnSamePlant = false;


        public PlantingLocationCondition(PlantingLocationTypeId id)
        {
            switch (id)
            {
                case PlantingLocationTypeId.OnPlatOfNormal:
                    currentCellConditions.Add(CellTypeId.TileEmpty);
                    belowCellConditions.Add(CellTypeId.PlatOfNormal);
                    break;
                case PlantingLocationTypeId.OnPlat:
                    currentCellConditions.Add(CellTypeId.TileEmpty);
                    belowCellConditions.Add(CellTypeId.Plat);
                    break;
                case PlantingLocationTypeId.OnWaterSurface:
                    currentCellConditions.Add(CellTypeId.Water);
                    aboveCellConditions.Add(CellTypeId.TileEmpty);
                    break;
                case PlantingLocationTypeId.OnSamePlant_OnlyStack:
                    bannedSpawn = true;
                    canStackOnSamePlant = true;
                    break;
                case PlantingLocationTypeId.OnAnyPlant:
                    currentCellConditions.Add(CellTypeId.HasPlant);
                    break;
                case PlantingLocationTypeId.OnSleepingShroom:
                    canAndOnlyCanSpawnOnSleepingShroom = true;
                    break;
                case PlantingLocationTypeId.OnGravestone:
                    currentCellConditions.Add(CellTypeId.Gravestone); break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(id), id, null);
            }
        }

        public bool CheckSpawn(PlantDef plantDef, Cell currentCell = null, Cell belowCell = null, Cell aboveCell = null)
        {
            if (bannedSpawn) return false;

            // （硬性条件）当前槽位不能有植物
            var slot = PlantConfigReader.GetPlacementSlot(plantDef);
            if (currentCell.CellPlantData.HasPlant(slot)) return false;

            // OR Condition
            if (canAndOnlyCanSpawnOnSleepingShroom)
            {
                return currentCell.CellPlantData.GetPlant(PlacementSlot.Normal) is ISleepyShroom { IsAwake: false };
            }

            // 上中下Tile判定
            if (currentCellConditions.Count > 0)
            {
                if (currentCell == null || currentCellConditions.Any(cellTypeId => !currentCell.Is(cellTypeId)))
                {
                    return false;
                }
            }

            if (belowCellConditions.Count > 0)
            {
                if (belowCell == null || belowCellConditions.Any(cellTypeId => !belowCell.Is(cellTypeId)))
                {
                    return false;
                }
            }

            if (aboveCellConditions.Count > 0)
            {
                if (aboveCell == null || aboveCellConditions.Any(cellTypeId => !aboveCell.Is(cellTypeId)))
                {
                    return false;
                }
            }

            return true;
        }

        public bool CheckStack(PlantDef plantDef, Cell currentCell, Cell belowCell, Cell aboveCell)
        {
            if (canStackOnSamePlant)
            {
                if (currentCell.CellPlantData.GetPlant(plantDef) is ICanBeStackedOn plant && plant.CanStack(plantDef))
                    return true;
            }

            return false;
        }
    }

    public enum PlantingLocationTypeId
    {
        NotSet,

        // Tile Conditions
        OnPlatOfNormal, // 可以种在 PlatOfNormal 上
        OnPlat, // 可以种在 Plat 上
        OnWaterSurface, // 可以种在水面上
        OnGravestone, // 可以种在墓碑上

        // Plant Conditions
        OnSamePlant_OnlyStack, // 可以种在相同植物上（以Stack的形式）
        OnAnyPlant, // 可以种在任何植物上（以Spawn的形式）
        OnSleepingShroom, // 可以种在睡眠植物上（以Spawn的形式）
    }
}