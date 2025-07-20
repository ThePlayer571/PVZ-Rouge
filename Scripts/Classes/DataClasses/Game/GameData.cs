using TPL.PVZR.Classes.MazeMap;
using TPL.PVZR.Tools.Random;

namespace TPL.PVZR.Classes.DataClasses.Game
{
    public interface IGameData
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
    }
}