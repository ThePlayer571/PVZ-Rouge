using TPL.PVZR.Classes.DataClasses;
using TPL.PVZR.Classes.DataClasses.Game;
using TPL.PVZR.Classes.DataClasses.Game.Interfaces;
using TPL.PVZR.Classes.DataClasses.Level;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.Classes.MazeMap;
using TPL.PVZR.Helpers.New.DataReader;
using TPL.PVZR.Tools.Random;

namespace TPL.PVZR.Helpers.New.ClassCreator
{
    public static class GameCreator
    {
        public static GameData CreateTestGameData()
        {
            var seed = RandomHelper.Default.NextUnsigned();
            //
            var testMazeMapData = CreateMazeMapData(MazeMapId.DaveLawn, GameDifficulty.N0, seed);
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

            return new GameData(testMazeMapData, testInventoryData);
        }

        public static MazeMapData CreateMazeMapData(MazeMapId mazeMapId, GameDifficulty difficulty, ulong seed)
        {
            var definition = GameConfigReader.GetMazeMapDefinition(mazeMapId, difficulty);
            return new MazeMapData(definition, seed);
        }

        public static LevelData CreateLevelData(IGameData gameData, LevelId id)
        {
            var definition = GameConfigReader.GetLevelDefinition(id);
            return new LevelData(gameData, definition);
        }
    }
}