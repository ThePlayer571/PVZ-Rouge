using System.Collections;
using TPL.PVZR.Core;
using TPL.PVZR.Gameplay.Data;
using TPL.PVZR.Gameplay.Entities.Plants.Base;
using UnityEngine;

namespace TPL.PVZR.Gameplay.Entities.Plants
{
    public partial class SplitPea: PeashooterBase
    {
        protected override void Shoot()
        {
            _EntitySystem.CreateIPea(ProjectileIdentifier.Pea,FirePoint_1.position, direction);
            StartCoroutine(ShootTwoPea());
            base.Shoot();
        }

        private IEnumerator ShootTwoPea()
        {
            _EntitySystem.CreateIPea(ProjectileIdentifier.Pea,FirePoint_2.position, direction.Reverse());
            yield return new WaitForSeconds(0.2f);
            _EntitySystem.CreateIPea(ProjectileIdentifier.Pea,FirePoint_2.position, direction.Reverse());
        }

        protected override void DefaultAI()
        {
            if (peashooterState == PeashooterState.InCold)
            {
                shootTimer+=Time.deltaTime;
                if (shootTimer >= shootColdTime)
                {
                    peashooterState = PeashooterState.Ready;
                }
            }

            if (peashooterState == PeashooterState.Ready)
            {
                RaycastHit2D hit_1 = Physics2D.Raycast(transform.position, direction.ToVector2(), Global.peashooterRange,
                    LayerMask.GetMask("Zombie", "Barrier"));
                RaycastHit2D hit_2 = Physics2D.Raycast(transform.position, -direction.ToVector2(), Global.peashooterRange,
                    LayerMask.GetMask("Zombie", "Barrier"));
                if ((hit_1.collider && hit_1.collider.CompareTag("Zombie"))
                    || (hit_2.collider && hit_2.collider.CompareTag("Zombie"))
                    )
                {
                    Shoot();
                }
            }
        }
    }
}