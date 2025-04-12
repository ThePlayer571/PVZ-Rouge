using TPL.PVZR.Core.Save.Modules;

namespace TPL.PVZR.Core.Random
{
    public class GameRandom
    {
        public int seed { get; private set; }
        public int callCount { get; private set; }
        public GameRandom(int seed)
        {
            this.seed = seed;
        }
    }
    
    
    public static class RandomHelper
    {
        # region 属性
        public static DeterministicRandom Default => _defaultRandom;
        public static DeterministicRandom Game => _gameRandom;
        public static DeterministicRandom MazeMap => _mazeMapRandom;
        # endregion
        # region 静态方法

        public static void Load(GameSaveData saveData)
        {
            // _defaultRandom 是不重要的随机数，不需要初始化
            _gameRandom.RestoreState(saveData.gameRandomState);
        }

        public static void SetSeed()
        {
            _defaultRandom = new DeterministicRandom();
            _gameRandom = new DeterministicRandom();
        }
        public static void SetSeed(int seed)
        {
            _defaultRandom = new DeterministicRandom(seed);
            _gameRandom = new DeterministicRandom(seed);
        }
        # endregion
        
        # region 私有
        private static DeterministicRandom _defaultRandom = new DeterministicRandom();
        private static DeterministicRandom _gameRandom = new DeterministicRandom();
        private static DeterministicRandom _mazeMapRandom = new DeterministicRandom();
        # endregion
        
    }
}