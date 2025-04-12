using System.Collections.Generic;
using System.Linq;
using TPL.PVZR.Core.Random;
using TPL.PVZR.Gameplay.Class.Levels;

namespace TPL.PVZR.Gameplay.Class.MazeMap.Core
{
    public abstract class MapConfig
    {
        // Behavior
        public abstract int rowCount { get; }
        public abstract int colCount { get; }

        protected abstract int levelCount { get; }
        public int finalLevel => levelCount;
    }

    public abstract class LevelConfig
    {
        public abstract List<LevelIdentifier> EasyLevels { get; }
        public abstract List<LevelIdentifier> NormalLevels { get; }
        public abstract List<LevelIdentifier> HardLevels { get; }
        public abstract List<int> EasyLevelsCount { get; }
        public abstract List<int> NormalLevelsCount { get; }
        public abstract List<int> HardLevelsCount { get; }

        
        protected List<LevelIdentifier> remainingEasyLevels = new();
        public virtual LevelIdentifier GetRandomEasyLevel()
        {
            if (!remainingEasyLevels.Any())
            {
                remainingEasyLevels.AddRange(EasyLevels);
            }
            return RandomHelper.Game.RandomPop(remainingEasyLevels);
        }
        
        protected List<LevelIdentifier> remainingNormalLevels = new();
        public virtual LevelIdentifier GetRandomNormalLevel()
        {
            if (!remainingNormalLevels.Any())
            {
                remainingNormalLevels.AddRange(NormalLevels);
            }
            return RandomHelper.Game.RandomPop(remainingNormalLevels);
        }
        
        protected List<LevelIdentifier> remainingHardLevels = new();
        public virtual LevelIdentifier GetRandomHardLevel()
        {
            if (!remainingHardLevels.Any())
            {
                remainingHardLevels.AddRange(HardLevels);
            }
            return RandomHelper.Game.RandomPop(remainingHardLevels);
        }
    }
}