using QFramework;
using TPL.PVZR.Models;
using TPL.PVZR.Systems;

namespace TPL.PVZR
{
    public class PVZRouge : Architecture<PVZRouge>
    {
        protected override void Init()
        {
            // ===== Others =====
            this.RegisterModel<IPhaseModel>(new PhaseModel());
            this.RegisterSystem<IMainGameSystem>(new MainGameSystem());
        }
    }
}