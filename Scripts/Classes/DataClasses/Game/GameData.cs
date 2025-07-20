using TPL.PVZR.Classes.MazeMap;
using TPL.PVZR.Tools.Random;
using TPL.PVZR.Tools.Save;

namespace TPL.PVZR.Classes.DataClasses.Game
{
    public interface IGameData : ISavable<GameSaveData>
    {
        IMazeMapData MazeMapData { get; set; }
        IInventoryData InventoryData { get; set; }
        GlobalEntityData GlobalEntityData { get; set; }
        ulong Seed { get; }
        DeterministicRandom Random { get; }
    }

    public class GameData : IGameData
    {
        public IMazeMapData MazeMapData { get; set; }
        public IInventoryData InventoryData { get; set; }
        public GlobalEntityData GlobalEntityData { get; set; }
        public ulong Seed { get; }
        public DeterministicRandom Random { get; }


        //

        public GameData(IMazeMapData mazeMapData, IInventoryData inventoryData, ulong seed)
        {
            this.MazeMapData = mazeMapData;
            this.InventoryData = inventoryData;
            this.GlobalEntityData = new GlobalEntityData();
            this.Seed = seed;
            Random = DeterministicRandom.Create(seed);
        }

        public GameData(GameSaveData saveData)
        {
            MazeMapData = new MazeMapData(saveData.mazeMapSaveData);
            InventoryData = new InventoryData(saveData.inventorySaveData);
            GlobalEntityData = saveData.globalEntityData;
            Seed = saveData.seed;
            Random = DeterministicRandom.Create(saveData.randomState);
        }

        public GameSaveData ToSaveData()
        {
            return new GameSaveData
            {
                mazeMapSaveData = MazeMapData.ToSaveData(),
                inventorySaveData = InventoryData.ToSaveData(),
                globalEntityData = GlobalEntityData,
                seed = Seed,
                randomState = Random.SaveState(),
            };
        }
    }
}