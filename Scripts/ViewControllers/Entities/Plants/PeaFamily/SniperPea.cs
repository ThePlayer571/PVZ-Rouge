using System.Linq;
using QFramework;
using TPL.PVZR.Classes;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.Helpers;
using TPL.PVZR.Helpers.New.GameObjectFactory;
using TPL.PVZR.Helpers.New.Methods;
using TPL.PVZR.Tools;
using TPL.PVZR.ViewControllers.Entities.Plants.Base;
using TPL.PVZR.ViewControllers.Entities.Zombies.Base;
using TPL.PVZR.ViewControllers.Entities.Zombies.Instances;
using UnityEngine;
using Time = UnityEngine.Time;

namespace TPL.PVZR.ViewControllers.Entities.Plants
{
    public sealed partial class SniperPea : Plant
    {
        public override PlantDef Def { get; } = new PlantDef(PlantId.SniperPea, PlantVariant.V0);

        protected override void OnInit()
        {
            this.HealthPoint = GlobalEntityData.Plant_Default_Health;

            _timer = new Timer(GlobalEntityData.Plant_SniperPea_ShootInterval);
            _detectTimer = new Timer(Global.Plant_Peashooter_DetectInterval);
            _layerMask = LayerMask.GetMask("Zombie", "Barrier");
        }

        private Timer _timer;
        private Timer _detectTimer;
        private int _layerMask;

        [SerializeField] private Transform FirePoint;
        [SerializeField] private CollisionDetector ZombieDetector;

        private Zombie _target;

        protected override void OnUpdate()
        {
            _timer.Update(Time.deltaTime);
            _detectTimer.Update(Time.deltaTime);

            if (_detectTimer.Ready)
            {
                _detectTimer.Reset();
                if (ZombieDetector.HasTarget)
                {
                    _target = ZombieDetector.DetectedTargets
                        .OrderBy(other => Vector2.Distance(other.transform.position, FirePoint.position))
                        .FirstOrDefault(other =>
                            {
                                var hit = Physics2D.Raycast(FirePoint.position,
                                    (other.GetComponent<Zombie>().ZombieNode.HeadPos.position - FirePoint.position).normalized,
                                    GlobalEntityData.Plant_Peashooter_ShootDistance, _layerMask);
                                return hit.collider != null && hit.collider.CompareTag("Zombie");
                            }
                        )
                        ?.gameObject.GetComponent<Zombie>();
                }
            }

            if (_timer.Ready && _target != null)
            {
                _timer.Reset();
                var direction = (_target.ZombieNode.HeadPos.position - FirePoint.position);
                EntityFactory.ProjectileFactory.CreatePea(ProjectileId.SnipePea, direction, FirePoint.position);
            }
        }
    }

    public sealed partial class SniperPea
    {
        [Header("View")] [SerializeField] private Transform Head;

        protected override void OnViewUpdate()
        {
            if (_target == null) return;
            var direction = _target.ZombieNode.HeadPos.position - FirePoint.position;
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            // 公式是统计得出的
            if (-90 < angle && angle < 90)
            {
                Head.LocalScaleX(Direction.ToInt());
                Head.LocalEulerAnglesZ(angle * Direction.ToInt());
            }
            else
            {
                Head.LocalScaleX(-Direction.ToInt());
                Head.LocalEulerAnglesZ((angle + 180) * Direction.ToInt());
            }
        }
    }
}