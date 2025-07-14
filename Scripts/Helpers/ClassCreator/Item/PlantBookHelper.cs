using System.Collections.Generic;
using System.Linq;
using QAssetBundle;
using QFramework;
using TPL.PVZR.Classes;
using TPL.PVZR.Classes.DataClasses;
using TPL.PVZR.Classes.DataClasses.Item.PlantBook;
using TPL.PVZR.Classes.DataClasses.Loot;

namespace TPL.PVZR.Helpers.ClassCreator.Item
{
    public static class PlantBookHelper
    {
        static PlantBookHelper()
        {
            var resLoader = ResLoader.Allocate();
            _bookDefDict = new Dictionary<PlantBookId, PlantBookDefinition>
            {
                [PlantBookId.MungBeanBook] = resLoader.LoadSync<PlantBookDefinition>(Plantbookdefinition.BundleName,
                    Plantbookdefinition.PlantBookDefinition_MungBean),
            };
        }

        private static Dictionary<PlantBookId, PlantBookDefinition> _bookDefDict;

        public static PlantBookDefinition GetPlantBookDefinition(PlantBookId plantBookId)
        {
            if (_bookDefDict.TryGetValue(plantBookId, out var plantBookDefinition))
            {
                return plantBookDefinition;
            }
            else
            {
                throw new KeyNotFoundException($"未找到PlantBookId: {plantBookId}");
            }
        }
        
        public static PlantBookData CreatePlantBookData(PlantBookId id)
        {
            if (_bookDefDict.TryGetValue(id, out var plantBookDefinition))
            {
                return new PlantBookData(plantBookDefinition);
            }
            else
            {
                throw new KeyNotFoundException($"未找到PlantBookId: {id}");
            }
        }


        private static IInventoryData _inventoryData;

        public static void SetInventory(IInventoryData inventoryData)
        {
            _inventoryData = inventoryData;
        }

        public static PlantDef GetPlantDef(PlantId id)
        {
            var plantBookData = _inventoryData.PlantBooks.FirstOrDefault(book => book.Id == id);
            if (plantBookData == null) return new PlantDef(id, PlantVariant.V0);
            else return new PlantDef(id, plantBookData.Variant);
        }
    }
}