using QFramework;
using TPL.PVZR.Classes.DataClasses_InLevel.Attack;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.Helpers.New.ClassCreator;
using TPL.PVZR.ViewControllers.Entities.EntityBase.Interfaces;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Entities.Projectiles
{
    public sealed class ShortSpore : Projectile, IPeaLikeInit
    {
        public override ProjectileId Id { get; } = ProjectileId.ShortSpore;

        public void Initialize(Vector2 direction)
        {
            _Rigidbody2D.velocity = GlobalEntityData.Projectile_Pea_Speed * direction;
            
            _initialPosX = _Rigidbody2D.position.x;
        }

        private bool _attacked = false;
        private float _initialPosX;


        protected override void Update()
        {
            base.Update();
            if (Mathf.Abs(_Rigidbody2D.position.x - _initialPosX) > GlobalEntityData.Plant_PuffShroom_ShootDistance)
            {
                Kill();
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (_attacked) return;

            if (other.collider.IsInLayerMask(LayerMask.GetMask("Zombie")))
            {
                _attacked = true;
                var attackData = AttackCreator.CreateAttackData(AttackId.Spore). WithPunchDirectionX(_Rigidbody2D.velocity.x);
                other.collider.GetComponent<IAttackable>().TakeAttack(attackData);
            }

            Kill();
        }
    }
}