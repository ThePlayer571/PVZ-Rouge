using TPL.PVZR.Classes;
using TPL.PVZR.Classes.GameStuff;
using TPL.PVZR.Helpers;
using TPL.PVZR.Helpers.Factory;
using TPL.PVZR.Tools;
using UnityEngine;
using Time = UnityEngine.Time;

namespace TPL.PVZR.ViewControllers.Entities.Plants
{
    public class Peashooter : Plant
    {
        public override PlantId Id { get; } = PlantId.PeaShooter;

        public override void Initialize(Direction2 direction)
        {
            base.Initialize(direction);
            this.HealthPoint = GlobalEntityData.Plant_Default_Health;

            _timer = new Timer(GlobalEntityData.Plant_Peashooter_ShootInterval);
            _detectTimer = new Timer(GlobalEntityData.Plant_Peashooter_DetectInterval);
            _layerMask = LayerMask.GetMask("Zombie", "Barrier");
        }

        private Timer _timer;
        private Timer _detectTimer;
        private int _layerMask;

        [SerializeField] private Transform FirePoint;

        protected override void Update()
        {
            base.Update();
            //
            _timer.Update(Time.deltaTime);
            _detectTimer.Update(Time.deltaTime);

            if (_timer.Ready && _detectTimer.Ready)
            {
                _detectTimer.Reset();
                var hit = Physics2D.Raycast(FirePoint.position, Direction.ToVector2(),
                    GlobalEntityData.Plant_Peashooter_ShootDistance, _layerMask);

                if (hit.collider && hit.collider.CompareTag("Zombie"))
                {
                    ProjectileHelper.CreatePea(Direction);
                    _timer.Reset();
                }
            }
        }
    }
}