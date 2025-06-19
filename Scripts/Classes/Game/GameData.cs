namespace TPL.PVZR.Classes.Game
{
    public class GameData : IGameData
    {
        // MazeMap数据
        public ulong Seed { get; }

        // 玩家属性
        public int InitialSunPoint { get; set; }
        
        
        //

        public GameData(ulong seed, int initialSunPoint)
        {
            this.Seed = seed;
            this.InitialSunPoint = initialSunPoint;
        }
    }
}