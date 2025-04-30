using TPL.PVZR.Core.Random;
using TPL.PVZR.Gameplay.Entities.Plants.Base;

namespace TPL.PVZR.Gameplay.Entities.Plants
{
    public class CornPult : CabbagePultBase
    {
        protected override void Throw()
        {
            var val = RandomHelper.Default.Value;
            if (val < 0.2)
            {
                _EntitySystem.CreateICabbage(ProjectileIdentifier.Butter, transform.position, direction);
            }
            else
            {
                _EntitySystem.CreateICabbage(ProjectileIdentifier.Kernel, transform.position, direction);
            }
            base.Throw();
        }
    }
}