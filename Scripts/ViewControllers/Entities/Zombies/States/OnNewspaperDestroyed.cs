using QFramework;
using TPL.PVZR.ViewControllers.Entities.Zombies.Base;

namespace TPL.PVZR.ViewControllers.Entities.Zombies.States
{
    public class OnNewspaperDestroyedState : AbstractState<ZombieState, Zombie>
    {
        public OnNewspaperDestroyedState(FSM<ZombieState> fsm, Zombie target) : base(fsm, target)
        {
        }

        protected override void OnEnter()
        {
            mTarget.baseAttackData.MultiplyDamage(4);
            mTarget.baseSpeed *= 3f;
        }
    }
}