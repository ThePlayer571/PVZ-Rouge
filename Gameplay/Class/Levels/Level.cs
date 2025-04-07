namespace TPL.PVZR.Gameplay.Class.Levels
{

    public interface ILevel
    {
        MapConfig MapConfig { get; }
        WaveConfig WaveConfig { get; }
        ZombieSpawnConfig ZombieSpawnConfig { get; }
        LootConfig LootConfig { get; }
    }

    public abstract class Level : ILevel
    {
        public enum ZombieSpawnPositionId
        {
            Lawn,
            Roof
        }

        public abstract MapConfig MapConfig { get; }
        public abstract WaveConfig WaveConfig { get; }
        public abstract ZombieSpawnConfig ZombieSpawnConfig { get; }
        public abstract LootConfig LootConfig { get; }
    }
}