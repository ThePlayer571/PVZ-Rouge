using QFramework;
using TPL.PVZR.Gameplay.Entities.Plants.Base;

namespace TPL.PVZR.Gameplay.Entities.Plants
{
    public class MelonPult : CabbagePultBase
    {
        protected override void Throw()
        {
            _EntitySystem.CreateICabbage(ProjectileIdentifier.Watermelon, transform.position, direction);
            base.Throw();
        }
    }
}