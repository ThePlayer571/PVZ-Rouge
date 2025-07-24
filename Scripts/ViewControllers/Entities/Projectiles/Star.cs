using System;
using System.Linq;
using QFramework;
using TPL.PVZR.Classes.DataClasses_InLevel.Attack;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.Helpers.New.ClassCreator;
using TPL.PVZR.Helpers.New.Methods;
using TPL.PVZR.Tools;
using TPL.PVZR.ViewControllers.Entities.EntityBase.Interfaces;
using TPL.PVZR.ViewControllers.Entities.Zombies.Base;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Entities.Projectiles
{
    public sealed class Star : Projectile, IPeaLikeInit
    {
        public override ProjectileId Id { get; } = ProjectileId.Star;

        public void Initialize(Vector2 direction)
        {
            _Rigidbody2D.velocity = GlobalEntityData.Projectile_Pea_Speed * direction;
            zombieDetector.RecordTargets = true;

            _detectTimer = new Timer(Global.Projectile_Star_DetectInterval);
            _detectTimer.SetRemaining(0.4f); // 初始等一会儿再跟踪
        }

        private bool _attacked = false;
        private Timer _detectTimer;
        [SerializeField] private CollisionDetector zombieDetector;
        private const float turnSpeed = 2f; // 转向速度，值越大转向越快

        protected override void Update()
        {
            base.Update();
            //
            _detectTimer.Update(Time.deltaTime);
            if (_detectTimer.Ready)
            {
                _detectTimer.Reset();
                if (zombieDetector.HasTarget)
                {
                    var closest = zombieDetector.DetectedTargets.MinBy(other =>
                        Vector2.Distance(other.transform.position, transform.position));

                    var targetDirection =
                        (closest.GetComponent<Zombie>().ZombieNode.CorePos.position - transform.position).normalized;

                    // 平滑插值到目标方向
                    var currentDirection = _Rigidbody2D.velocity.normalized;
                    var newDirection = Vector2.Lerp(currentDirection, targetDirection,
                        turnSpeed * Global.Projectile_Star_DetectInterval);
                    _Rigidbody2D.velocity = GlobalEntityData.Projectile_Pea_Speed * newDirection.normalized;
                }
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (_attacked) return;

            if (other.collider.IsInLayerMask(LayerMask.GetMask("Zombie")))
            {
                _attacked = true;
                var attackData = AttackCreator.CreateAttackData(AttackId.Star).WithPunchFrom(transform.position);
                other.collider.GetComponent<IAttackable>().TakeAttack(attackData);
            }

            this.Remove();
        }
    }
}