namespace TPL.PVZR.Classes.Game
{
    public class GameData : IGameData
    {
        // MazeMap数据
        public ulong Seed { get; }

        
        
        //

        public GameData(ulong seed)
        {
            this.Seed = seed;
        }
    }
}