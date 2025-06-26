using System;
using System.Collections.Generic;
using QFramework;
using TPL.PVZR.Classes.Game;
using TPL.PVZR.Classes.GameStuff;
using TPL.PVZR.Classes.MazeMap;
using TPL.PVZR.Classes.MazeMap.Instances.DaveHouse;
using TPL.PVZR.Core.Random;

namespace TPL.PVZR.Helpers
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
                CardHelper.CreateCardData(PlantId.PeaShooter),
                CardHelper.CreateCardData(PlantId.PeaShooter),
                CardHelper.CreateCardData(PlantId.Sunflower),
                
            };

            return new GameData(testMazeMapData, testInventoryData);
        }
    }
}