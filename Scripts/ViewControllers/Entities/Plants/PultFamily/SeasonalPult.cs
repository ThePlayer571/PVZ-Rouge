using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.Helpers.New.GameObjectFactory;
using TPL.PVZR.Helpers.New.Methods;
using TPL.PVZR.Tools;
using TPL.PVZR.Tools.Random;
using TPL.PVZR.ViewControllers.Entities.Plants.Base;
using UnityEngine;
using Time = UnityEngine.Time;

namespace TPL.PVZR.ViewControllers.Entities.Plants
{
    public sealed class SeasonalPult : Plant
    {
        public override PlantDef Def { get; } = new PlantDef(PlantId.SeasonalPult, PlantVariant.V0);

        protected override void OnInit()
        {
            this.HealthPoint = GlobalEntityData.Plant_Default_Health;

            _timer = new Timer(GlobalEntityData.Plant_SeasonalPult_ThrowInterval);
            _detectTimer = new Timer(Global.Plant_Peashooter_DetectInterval);
            _layerMask = LayerMask.GetMask("Zombie");

            _direction = this.Direction.ToVector2() + Vector2.up * 1.7f;
        }

        private Timer _timer;
        private Timer _detectTimer;
        private int _layerMask;
        private Vector2 _direction;

        [SerializeField] private Transform FirePoint;

        protected override void OnUpdate()
        {
            _timer.Update(Time.deltaTime);
            _detectTimer.Update(Time.deltaTime);

            if (_timer.Ready && _detectTimer.Ready)
            {
                _detectTimer.Reset();

                var posLD = FirePoint.position + new Vector3(0, -30f);
                var posRU = FirePoint.position +
                            new Vector3(GlobalEntityData.Plant_CabbagePult_ThrowDistance * Direction.ToInt(), 30f);
                var target = Physics2D.OverlapArea(posLD, posRU, _layerMask);

                if (target)
                {
                    _timer.Reset();
                    var variant = RandomHelper.Default.Range(0, 6);
                    switch (variant)
                    {
                        case 0:
                            _ProjectileService.CreatePea(ProjectileId.Seasonal_ChineseCabbage, _direction,
                                FirePoint.position);
                            break;
                        case 1:
                            _ProjectileService.CreatePea(ProjectileId.Seasonal_Carrot, _direction, FirePoint.position);
                            break;
                        case 2:
                            _ProjectileService.CreatePea(ProjectileId.Seasonal_Tomato, _direction
                                , FirePoint.position);
                            break;
                        case 3:
                            _ProjectileService.CreatePea(ProjectileId.Seasonal_Eggplant, _direction
                                , FirePoint.position);
                            break;
                        case 4:
                            _ProjectileService.CreatePea(ProjectileId.Seasonal_Broccoli, _direction
                                , FirePoint.position);
                            _ProjectileService.CreatePea(ProjectileId.Seasonal_Broccoli, _direction
                                , FirePoint.position);
                            _ProjectileService.CreatePea(ProjectileId.Seasonal_Broccoli, _direction
                                , FirePoint.position);
                            _ProjectileService.CreatePea(ProjectileId.Seasonal_Broccoli, _direction
                                , FirePoint.position);
                            _ProjectileService.CreatePea(ProjectileId.Seasonal_Broccoli, _direction
                                , FirePoint.position);
                            break;
                        case 5:
                            _ProjectileService.CreatePea(ProjectileId.Seasonal_Potato, _direction
                                , FirePoint.position);
                            break;
                    }
                }
            }
        }
    }
}