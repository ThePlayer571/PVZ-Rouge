using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.Helpers.New.GameObjectFactory;
using TPL.PVZR.Helpers.New.Methods;
using TPL.PVZR.Tools;
using TPL.PVZR.ViewControllers.Entities.Plants.Base;
using UnityEngine;
using Time = UnityEngine.Time;

namespace TPL.PVZR.ViewControllers.Entities.Plants
{
    public sealed class Starfruit : Plant
    {
        public override PlantDef Def { get; } = new PlantDef(PlantId.Starfruit, PlantVariant.V0);

        protected override void OnInit()
        {
            this.HealthPoint = GlobalEntityData.Plant_Default_Health;

            _timer = new Timer(GlobalEntityData.Plant_Peashooter_ShootInterval);
            _detectTimer = new Timer(Global.Plant_Peashooter_DetectInterval);
            _layerMask = LayerMask.GetMask("Zombie");
        }

        private Timer _timer;
        private Timer _detectTimer;
        private int _layerMask;

        [SerializeField] private Transform FirePoint_1;
        [SerializeField] private Transform FirePoint_2;
        [SerializeField] private Transform FirePoint_3;
        [SerializeField] private Transform FirePoint_4;
        [SerializeField] private Transform FirePoint_5;

        protected override void OnUpdate()
        {
            _timer.Update(Time.deltaTime);
            _detectTimer.Update(Time.deltaTime);

            if (_timer.Ready && _detectTimer.Ready)
            {
                _detectTimer.Reset();
                var hit = Physics2D.OverlapCircle(transform.position, GlobalEntityData.Plant_Peashooter_ShootDistance,
                    _layerMask);

                if (hit)
                {
                    var direction_1 = Direction.ToVector2();
                    var factor = -Direction.ToInt();
                    _ProjectileService.CreatePea(ProjectileId.Star, direction_1, FirePoint_1.position);
                    _ProjectileService.CreatePea(ProjectileId.Star, direction_1.Rotate(90 * factor),
                        FirePoint_2.position);
                    _ProjectileService.CreatePea(ProjectileId.Star, direction_1.Rotate(180 * factor),
                        FirePoint_3.position);
                    _ProjectileService.CreatePea(ProjectileId.Star, direction_1.Rotate(270 * factor),
                        FirePoint_4.position);
                    _ProjectileService.CreatePea(ProjectileId.Star, direction_1.Rotate(315 * factor),
                        FirePoint_5.position);
                    _timer.Reset();
                }
            }
        }
    }
}