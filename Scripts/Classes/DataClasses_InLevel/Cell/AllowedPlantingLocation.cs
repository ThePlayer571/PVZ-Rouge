using System;
using System.Collections.Generic;
using System.Linq;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.Helpers.New.DataReader;

namespace TPL.PVZR.Classes.DataClasses_InLevel
{
    /// <summary>
    /// 以植物为第一人称，描述“我的种植位置应该满足的条件”
    /// </summary>
    public class PlantingLocationCondition
    {
        // spawn
        public bool bannedSpawn = false;
        public List<CellTypeId> currentCellConditions = new();
        public List<CellTypeId> belowCellConditions = new();

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
                case PlantingLocationTypeId.OnSamePlant_OnlyStack:
                    bannedSpawn = true;
                    canStackOnSamePlant = true;
                    break;
                case PlantingLocationTypeId.OnAnyPlant:
                    currentCellConditions.Add(CellTypeId.HasPlant);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(id), id, null);
            }
        }

        public bool CheckSpawn(PlantDef plantDef, Cell currentCell, Cell belowCell)
        {
            if (currentCellConditions.Any(cellTypeId => !currentCell.Is(cellTypeId))) return false;
            if (belowCellConditions.Any(cellTypeId => !belowCell.Is(cellTypeId))) return false;

            var slot = PlantConfigReader.GetPlacementSlot(plantDef);
            if (currentCell.CellPlantData.HasPlant(slot)) return false;

            return true;
        }

        public bool CheckStack(PlantDef plantDef, Cell currentCell, Cell belowCell)
        {
            return canStackOnSamePlant && currentCell.CellPlantData.HasPlant(plantDef);
        }
    }

    public enum PlantingLocationTypeId
    {
        NotSet,

        // Tile Conditions
        OnPlatOfNormal, // 可以种在 PlatOfNormal 上
        OnPlat, // 可以种在 Plat 上

        // Plant Conditions
        OnSamePlant_OnlyStack, // 可以种在相同植物上（以Stack的形式）
        OnAnyPlant, // 可以种在任何植物上（以Spawn的形式）
    }
}