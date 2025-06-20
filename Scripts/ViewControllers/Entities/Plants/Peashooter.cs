using Codice.Client.Common;
using TPL.PVZR.Classes;
using TPL.PVZR.Core;
using TPL.PVZR.Helpers;
using UnityEngine;
using Time = UnityEngine.Time;

namespace TPL.PVZR.ViewControllers.Entities.Plants
{
    public class Peashooter : Plant
    {
        public override void Initialize(Direction2 direction)
        {
            base.Initialize(direction);
            this.HealthPoint = GlobalEntityData.Plant_Default_Health;

            _timer = new Timer(GlobalEntityData.Plant_Peashooter_ShootInterval);
            _detectTimer = new Timer(GlobalEntityData.Plant_Peashooter_DetectInterval);
        }

        [SerializeField] private Timer _timer;
        [SerializeField] private Timer _detectTimer;

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
                    GlobalEntityData.Plant_Peashooter_ShootDistance, LayerMask.GetMask("Zombie", "Barrier"));
                if (hit.collider.CompareTag("Zombie"))
                {
                    ProjectileHelper.CreatePea(Direction);
                    _timer.Reset();
                }
            }
        }
    }
}