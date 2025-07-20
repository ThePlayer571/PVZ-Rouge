using TPL.PVZR.Classes.DataClasses.Game;
using TPL.PVZR.Tools.Save.SaveClasses;

namespace TPL.PVZR.Tools.Random
{
    public static class RandomHelper
    {
        public static DeterministicRandom Default { get; private set; } = DeterministicRandom.Create();
        public static DeterministicRandom Game => _gameData.Random;


        private static IGameData _gameData = null;

        public static void SetGame(IGameData gameData)
        {
            _gameData = gameData;
        }
    }
}