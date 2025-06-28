using QFramework;
using TPL.PVZR.Classes.ZombieAI.PathFinding;
using TPL.PVZR.Classes.ZombieAI.Public;
using TPL.PVZR.CommandEvents.Phase;
using TPL.PVZR.Models;
using UnityEngine;

namespace TPL.PVZR.Systems
{
    public interface IZombieAISystem : ISystem
    {
        IZombieAIUnit ZombieAIUnit { get; }
    }

    public class ZombieAISystem : AbstractSystem, IZombieAISystem
    {
        public IZombieAIUnit ZombieAIUnit { get; private set; }
        private ILevelGridModel _LevelGridModel;

        protected override void OnInit()
        {
            _LevelGridModel = this.GetModel<ILevelGridModel>();

            ZombieAIUnit = new ZombieAIUnit();

            this.RegisterEvent<OnPhaseChangeEvent>(e =>
            {
                switch (e.GamePhase)
                {
                    case GamePhase.LevelInitialization:
                        switch (e.PhaseStage)
                        {
                            case PhaseStage.EnterNormal:
                                ZombieAIUnit.InitializeFrom(_LevelGridModel.LevelMatrix);
                                break;
                        }
                        break;
                }
            });
        }
    }
}