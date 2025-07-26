using TPL.PVZR.Classes.DataClasses;
using TPL.PVZR.Classes.DataClasses.Award;
using TPL.PVZR.Classes.DataClasses.Game;
using TPL.PVZR.Classes.DataClasses.Level;
using TPL.PVZR.Classes.DataClasses.Loot;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.Classes.MazeMap;
using TPL.PVZR.Helpers.New.DataReader;
using TPL.PVZR.Tools.Random;
using TPL.PVZR.Tools.Save;

namespace TPL.PVZR.Helpers.New.ClassCreator
{
    public static class GameCreator
    {
        public static IGameData CreateGameData(GameDef gameDef, ulong seed)
        {
            var definition = GameConfigReader.GetGameDefinition(gameDef);
            //
            var mazeMapData = CreateMazeMapData(definition.MazeMapDef, seed);
            //
            var awardData = new AwardData(mazeMapData.InitialAwards);
            // 
            var inventoryData = new InventoryData();
            PlantDefHelper.SetInventory(inventoryData); // 临时inventoryData
            foreach (var lootInfo in definition.InventoryLoots)
            {
                var lootData = LootData.Create(lootInfo);
                inventoryData.AddLootAuto(lootData);
            }

            PlantDefHelper.SetInventory(null); // 临时inventoryData

            return new GameData(mazeMapData, inventoryData, awardData, seed);
        }

        public static IGameData CreateGameData(GameSaveData saveData)
        {
            return new GameData(saveData);
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