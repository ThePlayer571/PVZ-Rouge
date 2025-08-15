using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.ViewControllers.Entities.Plants.Base;

namespace TPL.PVZR.Classes.DataClasses_InLevel
{
    public class CellPlantData : IEnumerable<Plant>
    {
        #region Public

        public bool IsEmpty(params PlacementSlot[] slots)
        {
            return slots.Select(GetPlant).All(plant => plant == null);
        }

        public bool IsEmpty()
        {
            return Normal == null && Overlay == null && Air == null && Behind == null;
        }

        public Plant GetPlantToShovelFirst()
        {
            if (Air != null) return Air;
            if (Normal != null) return Normal;
            if (Overlay != null) return Overlay;
            if (Behind != null) return Behind;
            return null;
        }

        public Plant GetPlant(PlantDef def)
        {
            return this.FirstOrDefault(plant => plant.Def == def);
        }

        public Plant GetPlant(PlacementSlot placement)
        {
            switch (placement)
            {
                case PlacementSlot.Normal:
                    return Normal;
                case PlacementSlot.Overlay:
                    return Overlay;
                case PlacementSlot.Air:
                    return Air;
                case PlacementSlot.Behind:
                    return Behind;
                default:
                    throw new System.ArgumentOutOfRangeException(nameof(placement), placement, null);
            }
        }

        public Plant SetPlant(Plant plant, PlacementSlot placement)
        {
            switch (placement)
            {
                case PlacementSlot.Normal:
                    Normal = plant;
                    break;
                case PlacementSlot.Overlay:
                    Overlay = plant;
                    break;
                case PlacementSlot.Air:
                    Air = plant;
                    break;
                case PlacementSlot.Behind:
                    Behind = plant;
                    break;
            }

            return plant;
        }

        public bool HasPlant(PlantId plantId = PlantId.NotSet)
        {
            if (plantId == PlantId.NotSet)
            {
                return !IsEmpty();
            }
            else
            {
                return (Normal != null && Normal.Def.Id == plantId) ||
                       (Overlay != null && Overlay.Def.Id == plantId) ||
                       (Air != null && Air.Def.Id == plantId) ||
                       (Behind != null && Behind.Def.Id == plantId);
            }
        }

        public bool HasPlant(PlacementSlot slot, PlantId plantId = PlantId.NotSet)
        {
            var plant = GetPlant(slot);
            if (plantId == PlantId.NotSet)
            {
                return plant != null;
            }
            else
            {
                return plant != null && plant.Def.Id == plantId;
            }
        }

        #endregion

        private Plant Normal;
        private Plant Overlay;
        private Plant Air;
        private Plant Behind;


        public IEnumerator<Plant> GetEnumerator()
        {
            if (Normal != null) yield return Normal;
            if (Overlay != null) yield return Overlay;
            if (Air != null) yield return Air;
            if (Behind != null) yield return Behind;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}