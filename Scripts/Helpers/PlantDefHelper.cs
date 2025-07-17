using System.Linq;
using TPL.PVZR.Classes;
using TPL.PVZR.Classes.DataClasses;
using TPL.PVZR.Classes.InfoClasses;

namespace TPL.PVZR.Helpers.New
{
    public static class PlantDefHelper
    {
        private static IInventoryData _inventoryData;

        public static void SetInventory(IInventoryData inventoryData)
        {
            _inventoryData = inventoryData;
        }

        public static PlantDef ToDef(this PlantId id)
        {
            var plantBookData = _inventoryData.PlantBooks.FirstOrDefault(book => book.Id == id);
            if (plantBookData == null) return new PlantDef(id, PlantVariant.V0);
            else return new PlantDef(id, plantBookData.Variant);
        }
    }
}