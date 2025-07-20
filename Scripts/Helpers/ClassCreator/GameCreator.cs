using TPL.PVZR.Classes.DataClasses;
using TPL.PVZR.Classes.DataClasses.Game;
using TPL.PVZR.Classes.DataClasses.Level;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.Classes.MazeMap;
using TPL.PVZR.Helpers.New.DataReader;
using TPL.PVZR.Tools.Random;

namespace TPL.PVZR.Helpers.New.ClassCreator
{
    public static class GameCreator
    {
        public static IGameData CreateTestGameData(ulong seed)
        {
            var testMazeMapData =
                GameCreator.CreateMazeMapData(new MazeMapDef { Id = MazeMapId.DaveLawn, Difficulty = GameDifficulty.N0 }, seed);
            
            IInventoryData testInventoryData = new InventoryData();
            testInventoryData.AddCard(ItemCreator.CreateCardData(new PlantDef(PlantId.PeaShooter, PlantVariant.V0),
                locked: true));
            testInventoryData.AddCard(ItemCreator.CreateCardData(new PlantDef(PlantId.Sunflower, PlantVariant.V0),
                locked: true));
            testInventoryData.AddCard(ItemCreator.CreateCardData(new PlantDef(PlantId.Flowerpot, PlantVariant.V0),
                locked: true));
            testInventoryData.AddCard(ItemCreator.CreateCardData(new PlantDef(PlantId.Marigold, PlantVariant.V0),
                locked: true));
            testInventoryData.AddCard(ItemCreator.CreateCardData(new PlantDef(PlantId.Cactus, PlantVariant.V0),
                locked: false));
            testInventoryData.AddCard(ItemCreator.CreateCardData(new PlantDef(PlantId.Wallnut, PlantVariant.V0),
                locked: false));
            testInventoryData.AddCard(ItemCreator.CreateCardData(new PlantDef(PlantId.PeaShooter, PlantVariant.V0),
                locked: false));
            testInventoryData.AddCard(ItemCreator.CreateCardData(new PlantDef(PlantId.PeaShooter, PlantVariant.V0),
                locked: false));

            return new GameData(testMazeMapData, testInventoryData, seed);
        }

        public static IMazeMapData CreateMazeMapData(MazeMapDef mazeMapDef, ulong seed)
        {
            var definition = GameConfigReader.GetMazeMapDefinition(mazeMapDef);
            return new MazeMapData(definition, seed);
        }

        public static ILevelData CreateLevelData(IGameData gameData, LevelDef def)
        {
            var definition = GameConfigReader.GetLevelDefinition(def);
            return new LevelData(gameData, definition);
        }
    }
}