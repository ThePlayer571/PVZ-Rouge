using TPL.PVZR.Classes.DataClasses.Item.Card;
using TPL.PVZR.Classes.DataClasses.Item.PlantBook;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.Helpers.New.DataReader;

namespace TPL.PVZR.Helpers.New.ClassCreator
{
    public static class ItemCreator
    {
        public static CardData CreateCardData(PlantDef plantDef, bool locked = false)
        {
            var definition = PlantConfigReader.GetCardDefinition(plantDef);
            return new CardData(definition, locked);
        }

        public static PlantBookData CreatePlantBookData(PlantBookId plantBookId)
        {
            var definition = PlantBookConfigReader.GetPlantBookDefinition(plantBookId);
            return new PlantBookData(definition);
        }
    }
}