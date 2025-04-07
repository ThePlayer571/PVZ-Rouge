using QFramework;
using TPL.PVZR.Architecture.Systems;
using TPL.PVZR.Architecture.Systems.PhaseSystems;

namespace TPL.PVZR.Architecture.Commands
{
    public class DaveDieCommand:AbstractCommand
    {
        protected override void OnExecute()
        {
            this.GetSystem<IGamePhaseSystem>().ChangePhase(GamePhaseSystem.GamePhase.Defeat);
        }
    }
}