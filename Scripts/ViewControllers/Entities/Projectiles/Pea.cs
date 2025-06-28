using QFramework;
using TPL.PVZR.Classes.LevelStuff;
using TPL.PVZR.Tools;
using TPL.PVZR.ViewControllers.Entities.Zombies.Base;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Entities.Projectiles
{
    public sealed class Pea : Projectile, IPeaLikeInit
    {
        public void Initialize(Direction2 direction)
        {
            _Rigidbody.velocity = GlobalEntityData.Projectile_Pea_Speed * direction.ToVector2(); 
        }

        protected override void Awake()
        {
            base.Awake();

            this.OnCollisionEnter2DEvent.Register(other =>
            {
                if (other.collider.CompareTag("Zombie"))
                {
                    other.collider.GetComponent<Zombie>().TakeAttack(new AttackData(GlobalEntityData.Projectile_Pea_Damage, GlobalEntityData.Projectile_Pea_PunchForce, false));
                }
                
                this.Remove();
            }).UnRegisterWhenGameObjectDestroyed(this);
        }
    }
}