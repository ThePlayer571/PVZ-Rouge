using QFramework;
using TPL.PVZR.ViewControllers.Entities.Zombies.Base;

namespace TPL.PVZR.ViewControllers.Entities.Zombies.States
{
    public class DeadState : AbstractState<ZombieState, Zombie>
    {
        public DeadState(FSM<ZombieState> fsm, Zombie target) : base(fsm, target)
        {
        }
    }
}