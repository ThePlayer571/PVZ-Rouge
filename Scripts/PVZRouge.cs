using QFramework;
using TPL.PVZR.Models;
using TPL.PVZR.Systems;

namespace TPL.PVZR
{
    public class PVZRouge : Architecture<PVZRouge>
    {
        protected override void Init()
        {
            // ===== Game =====
            this.RegisterSystem<IMainGameSystem>(new MainGameSystem());
            this.RegisterSystem<IGameSystem>(new GameSystem());
            this.RegisterModel<IGameModel>(new GameModel());

            // ===== Level =====
            this.RegisterModel<ILevelModel>(new LevelModel());
            this.RegisterModel<ILevelGridModel>(new LevelGridModel());
            this.RegisterSystem<ILevelSystem>(new LevelSystem());
            this.RegisterSystem<ISunSystem>(new SunSystem());
            this.RegisterSystem<IHandSystem>(new HandSystem());
            this.RegisterSystem<ISeedSystem>(new SeedSystem());
            this.RegisterSystem<IZombieAISystem>(new ZombieAISystem());
            this.RegisterSystem<IZombieSpawnSystem>(new ZombieSpawnSystem());
            this.RegisterSystem<IWaveSystem>(new WaveSystem());
            this.RegisterSystem<IEnvironmentSystem>(new EnvironmentSystem());

            // ===== Others =====
            this.RegisterModel<IPhaseModel>(new PhaseModel());
        }
    }
}