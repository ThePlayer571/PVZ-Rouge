using TPL.PVZR.Classes;
using TPL.PVZR.Classes.DataClasses.Game;
using TPL.PVZR.Classes.DataClasses.Item.Card;
using TPL.PVZR.Helpers.New.DataReader;

namespace TPL.PVZR.Helpers.New
{
    public static class CardCreator
    {
        public static CardData CreateCardData(PlantDef plantDef, bool locked = false)
        {
            var definition = PlantConfigReader.GetCardDefinition(plantDef);
            return new CardData(definition, locked);
        }
    }

    public static class GameCreator
    {
        public static GameData CreateTestGameData()
        {
            
        }
    }
}