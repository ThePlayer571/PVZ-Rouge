using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.Helpers.New.GameObjectFactory;
using TPL.PVZR.Helpers.New.Methods;
using TPL.PVZR.Tools;
using TPL.PVZR.ViewControllers.Entities.Plants.Base;
using UnityEngine;
using Time = UnityEngine.Time;

namespace TPL.PVZR.ViewControllers.Entities.Plants
{
    public sealed class MelonPult : Plant
    {
        public override PlantDef Def { get; } = new PlantDef(PlantId.MelonPult, PlantVariant.V0);

        protected override void OnInit()
        {
            this.HealthPoint = GlobalEntityData.Plant_Default_Health;

            _timer = new Timer(GlobalEntityData.Plant_CabbagePult_ThrowInterval);
            _detectTimer = new Timer(Global.Plant_CabbagePult_DetectInterval);
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
                    _ProjectileService.CreatePea(ProjectileId.Melon, _direction, FirePoint.position);
                }
            }
        }
    }
}