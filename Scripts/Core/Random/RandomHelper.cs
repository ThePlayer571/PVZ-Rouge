namespace TPL.PVZR.Core.Random
{
    public static class RandomHelper
    {
        # region 属性

        public static DeterministicRandom Default { get; private set; } = new DeterministicRandom();
        public static DeterministicRandom Game { get; private set; } = new DeterministicRandom();

        # endregion

        # region 初始化方法

        public static void Load(GameSaveData saveData)
        {
            // _defaultRandom 是不重要的随机数，不需要初始化
            Game.RestoreState(saveData.gameRandomState);
        }

        public static void SetRandomSeed()
        {
            Default = new DeterministicRandom();
            Game = new DeterministicRandom();
        }

        public static void SetSeed(int seed)
        {
            Default = new DeterministicRandom(seed);
            Game = new DeterministicRandom(seed);
        }

        # endregion

        #region 为MazeMap生成单独开辟的

        public static DeterministicRandom MazeMap { get; } = new DeterministicRandom();

        #endregion
    }
}