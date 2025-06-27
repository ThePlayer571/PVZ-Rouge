using QFramework;
using TPL.PVZR.Events;
using TPL.PVZR.Gameplay.Class.ZombieAI.PathFinding;
using TPL.PVZR.Gameplay.Class.ZombieAI.Public;
using TPL.PVZR.Models;

namespace TPL.PVZR.Systems
{
    public interface IZombieAISystem : ISystem
    {
    }

    public class ZombieAISystem : AbstractSystem, IZombieAISystem
    {
        private IZombieAIUnit _zombieAIUnit;
        private ILevelGridModel _LevelGridModel;

        protected override void OnInit()
        {
            _LevelGridModel = this.GetModel<ILevelGridModel>();

            _zombieAIUnit = new ZombieAIUnit();

            this.RegisterEvent<OnPhaseChangeEvent>(e =>
            {
                switch (e.GamePhase)
                {
                    case GamePhase.LevelInitialization:
                        switch (e.PhaseStage)
                        {
                            case PhaseStage.EnterNormal:
                                "call AI System".LogInfo();
                                $"_LevelGridModel.LevelMatrix is null: {_LevelGridModel.LevelMatrix is null}".LogInfo();
                                _zombieAIUnit.InitializeFrom(_LevelGridModel.LevelMatrix);
                                _zombieAIUnit.DebugDisplayMatrix();
                                break;
                        }
                        break;
                }
            });
        }
    }
}