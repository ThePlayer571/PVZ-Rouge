using TPL.PVZR.Tools;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Entities.Projectiles
{
    public sealed class Pea : Projectile
    {
        public void Initialize(Direction2 direction)
        {
            _Rigidbody.velocity = direction.ToVector2();
            // _Rigidbody.velocity = GlobalEntityData.Projectile_Pea_Speed * direction.ToVector2();
        }
    }
}