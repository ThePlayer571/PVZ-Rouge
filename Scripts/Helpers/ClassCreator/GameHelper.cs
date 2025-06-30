using System.Collections.Generic;
using TPL.PVZR.Classes;
using TPL.PVZR.Classes.DataClasses;
using TPL.PVZR.Classes.DataClasses.Card;
using TPL.PVZR.Classes.DataClasses.Game;
using TPL.PVZR.Classes.MazeMap;
using TPL.PVZR.Tools.Random;

namespace TPL.PVZR.Helpers.ClassCreator
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
                CardHelper.CreateCardData(PlantId.Flowerpot),
                CardHelper.CreateCardData(PlantId.Sunflower),
                CardHelper.CreateCardData(PlantId.PeaShooter),
                CardHelper.CreateCardData(PlantId.SnowPea),
                CardHelper.CreateCardData(PlantId.Wallnut),
            };

            return new GameData(testMazeMapData, testInventoryData);
        }
    }
}