using System.Collections.Generic;
using TPL.PVZR.Classes.Game;
using TPL.PVZR.Classes.GameStuff;
using TPL.PVZR.Classes.MazeMap;
using TPL.PVZR.Tools.Random;

namespace TPL.PVZR.Helpers.Factory
{
    public static class GameHelper
    {
        public static GameData CreateGameData(ulong? seed = null)
        {
            seed ??= RandomHelper.Default.NextUnsigned();
            //
            var testMazeMapData = MazeMapHelper.CreateMazeMapData(MazeMapIdentifier.DaveHouse, seed.Value);
            var testInventoryData = new InventoryData();
            testInventoryData.Cards = new List<CardData>()
            {
                CardHelper.CreateCardData(PlantId.PeaShooter),
                CardHelper.CreateCardData(PlantId.Flowerpot),
                CardHelper.CreateCardData(PlantId.Wallnut),
                CardHelper.CreateCardData(PlantId.Sunflower),
            };

            return new GameData(testMazeMapData, testInventoryData);
        }
    }
}