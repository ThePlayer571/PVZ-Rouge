using QFramework;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.Helpers.New.GameObjectFactory;
using TPL.PVZR.Helpers.New.Methods;
using TPL.PVZR.Tools;
using TPL.PVZR.ViewControllers.Entities.Plants.Base;
using UnityEngine;
using Time = UnityEngine.Time;

namespace TPL.PVZR.ViewControllers.Entities.Plants
{
    public sealed class Repeater : Plant
    {
        public override PlantDef Def { get; } = new PlantDef(PlantId.Repeater, PlantVariant.V0);

        protected override void OnInit()
        {
            this.HealthPoint = GlobalEntityData.Plant_Default_Health;

            _timer = new Timer(GlobalEntityData.Plant_Peashooter_ShootInterval);
            _detectTimer = new Timer(Global.Plant_Peashooter_DetectInterval);
            _layerMask = LayerMask.GetMask("Zombie", "Barrier");
        }

        private Timer _timer;
        private Timer _detectTimer;
        private int _layerMask;

        [SerializeField] private Transform FirePoint;

        protected override void OnUpdate()
        {
            _timer.Update(Time.deltaTime);
            _detectTimer.Update(Time.deltaTime);

            if (_timer.Ready && _detectTimer.Ready)
            {
                _detectTimer.Reset();
                var hit = Physics2D.Raycast(FirePoint.position, Direction.ToVector2(),
                    GlobalEntityData.Plant_Peashooter_ShootDistance, _layerMask);

                if (hit.collider && hit.collider.CompareTag("Zombie"))
                {
                    ActionKit.Sequence()
                        .Callback(() =>
                            EntityFactory.ProjectileFactory.CreatePea(ProjectileId.Pea, Direction.ToVector2(),
                                FirePoint.position))
                        .Delay(GlobalEntityData.Plant_Repeater_PeaInterval)
                        .Callback(() =>
                            EntityFactory.ProjectileFactory.CreatePea(ProjectileId.Pea, Direction.ToVector2(),
                                FirePoint.position))
                        .Start(this);

                    _timer.Reset();
                }
            }
        }
    }
}