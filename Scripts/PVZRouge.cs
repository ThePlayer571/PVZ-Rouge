using QFramework;
using TPL.PVZR.Helpers.New.GameObjectFactory;
using TPL.PVZR.Models;
using TPL.PVZR.Services;
using TPL.PVZR.Systems;
using TPL.PVZR.Systems.Level_Data;
using TPL.PVZR.Systems.Level_Event;
using TPL.PVZR.Systems.MazeMap;

namespace TPL.PVZR
{
    public class PVZRouge : Architecture<PVZRouge>
    {
        protected override void Init()
        {
            // ===== Game =====
            this.RegisterSystem<IMainGameSystem>(new MainGameSystem());
            this.RegisterSystem<IConfigLoadSystem>(new ConfigLoadSystem());
            this.RegisterSystem<IGameSystem>(new GameSystem());
            this.RegisterModel<IGameModel>(new GameModel());
            this.RegisterSystem<IMazeMapSystem>(new MazeMapSystem());
            this.RegisterSystem<IInventorySystem>(new InventorySystem());
            this.RegisterSystem<IAwardSystem>(new AwardSystem());
            this.RegisterSystem<IRecipeStoreSystem>(new RecipeStoreSystem());
            this.RegisterSystem<ICoinStoreSystem>(new CoinStoreSystem());
            this.RegisterSystem<ISellStoreSystem>(new SellStoreSystem());
            this.RegisterSystem<ISaveSystem>(new SaveSystem());
            this.RegisterSystem<ILevelGridSystem>(new LevelGridSystem());
            this.RegisterSystem<IGravestoneSystem>(new GravestoneSystem());
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
            this.RegisterService<IPhaseService>(new PhaseService());
            this.RegisterService<IGamePhaseChangeService>(new GamePhaseChangeService());
            this.RegisterService<ISaveService>(new SaveService());
            this.RegisterService<IPlantService>(new PlantService());
            this.RegisterService<IZombieService>(new ZombieService());
        }
    }
}