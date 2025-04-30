using System.Collections;
using UnityEngine;
using TPL.PVZR.Gameplay.Entities.Plants.Base;

namespace TPL.PVZR.Gameplay.Entities.Plants
{
    public sealed partial class RepeaterPea:PeashooterBase
    {
        protected override void Shoot()
        {
            StartCoroutine(ShootTwoPea());
            base.Shoot();
        }

        private IEnumerator ShootTwoPea()
        {
            _EntitySystem.CreateIPea(ProjectileIdentifier.Pea,FirePoint.position, direction);
            yield return new WaitForSeconds(0.2f);
            _EntitySystem.CreateIPea(ProjectileIdentifier.Pea,FirePoint.position, direction);
        }
    }
}