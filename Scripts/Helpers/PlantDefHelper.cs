using System;
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
            if (inventoryData == null)
            {
                _inventoryData = inventoryData;
            }
            else
            {
                if (_inventoryData != null)
                {
                    throw new InvalidOperationException("已设置过 inventoryData，不能再次设置");
                }
                else
                {
                    _inventoryData = inventoryData;
                }
            }
        }

        public static PlantDef ToDef(this PlantId id, IInventoryData inventoryData = null)
        {
            inventoryData ??= _inventoryData;
            if (inventoryData == null) throw new NullReferenceException("未进行 SetInventory()，也未传入临时 inventoryData");

            var plantBookData = inventoryData.PlantBooks.FirstOrDefault(book => book.PlantId == id);
            if (plantBookData == null) return new PlantDef(id, PlantVariant.V0);
            else return new PlantDef(id, plantBookData.Variant);
        }
    }
}