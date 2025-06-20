using UnityEngine;

namespace TPL.PVZR.ViewControllers.Entities.Projectiles
{
    public abstract class Projectile : Entity
    {
        protected override void Awake()
        {
            base.Awake();
            _Rigidbody = this.GetComponent<Rigidbody2D>();
        }

        protected Rigidbody2D _Rigidbody { get; private set; }
    }
}