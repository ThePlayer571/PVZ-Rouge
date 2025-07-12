using QFramework;
using TPL.PVZR.Classes.DataClasses.Attack;
using TPL.PVZR.Helpers.ClassCreator;
using TPL.PVZR.Tools;
using TPL.PVZR.ViewControllers.Entities.Zombies.Base;

namespace TPL.PVZR.ViewControllers.Entities.Projectiles
{
    public class MungBean : Projectile, IPeaLikeInit
    {
        
        public void Initialize(Direction2 direction)
        {
            _Rigidbody.velocity = GlobalEntityData.Projectile_Pea_Speed * direction.ToVector2();
        }

        private bool _attacked = false;

        protected override void Awake()
        {
            base.Awake();

            this.OnCollisionEnter2DEvent.Register(other =>
            {
                if (_attacked) return;

                if (other.collider.CompareTag("Zombie"))
                {
                    _attacked = true;
                    var attackData = AttackHelper.CreateAttackData(AttackId.MungBean).WithPunchFrom(transform.position);
                    other.collider.GetComponent<Zombie>().TakeAttack(attackData);
                }

                this.Remove();
            }).UnRegisterWhenGameObjectDestroyed(this);
        }
    }
}