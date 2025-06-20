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
            this.RegisterSystem<ILevelSystem>(new LevelSystem());
            this.RegisterSystem<IHandSystem>(new HandSystem());

            // ===== Others =====
            this.RegisterModel<IPhaseModel>(new PhaseModel());
        }
    }
}