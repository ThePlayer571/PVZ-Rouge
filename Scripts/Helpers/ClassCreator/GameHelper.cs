using System.Collections.Generic;
using TPL.PVZR.Classes;
using TPL.PVZR.Classes.DataClasses;
using TPL.PVZR.Classes.DataClasses.Game;
using TPL.PVZR.Classes.DataClasses.Item.Card;
using TPL.PVZR.Classes.MazeMap;
using TPL.PVZR.Helpers.ClassCreator.Item;
using TPL.PVZR.Tools.Random;

namespace TPL.PVZR.Helpers.ClassCreator
{
    public static class GameHelper
    {
        public static GameData CreateGameData(ulong? seed = null)
        {
            seed ??= RandomHelper.Default.NextUnsigned();
            //
            var testMazeMapData = MazeMapHelper.CreateMazeMapData(MazeMapId.DaveLawn, GameDifficulty.N0, seed.Value);
            IInventoryData testInventoryData = new InventoryData();
            testInventoryData.AddCard(CardHelper.CreateCardData(new PlantDef(PlantId.PeaShooter, PlantVariant.V0), locked: true));
            testInventoryData.AddCard(CardHelper.CreateCardData(new PlantDef(PlantId.PeaShooter, PlantVariant.V0), locked: false));
            testInventoryData.AddCard(CardHelper.CreateCardData(new PlantDef(PlantId.Sunflower, PlantVariant.V0), locked: true));
            testInventoryData.AddCard(CardHelper.CreateCardData(new PlantDef(PlantId.Flowerpot, PlantVariant.V0), locked: true));
            testInventoryData.AddCard(CardHelper.CreateCardData(new PlantDef(PlantId.Marigold, PlantVariant.V0), locked: true));

            return new GameData(testMazeMapData, testInventoryData);
        }
    }
}