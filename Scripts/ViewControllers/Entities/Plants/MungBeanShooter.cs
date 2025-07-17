using QFramework;
using TPL.PVZR.Classes;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.Helpers;
using TPL.PVZR.Helpers.New.GameObjectFactory;
using TPL.PVZR.Helpers.New.Methods;
using TPL.PVZR.Tools;
using TPL.PVZR.ViewControllers.Entities.Plants.Base;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Entities.Plants
{
    public  sealed class MungBeanShooter : Plant
    {
        public override PlantDef Def { get; } = new PlantDef(PlantId.PeaShooter, PlantVariant.V1);


        protected override void OnInit()
        {
            this.HealthPoint = GlobalEntityData.Plant_Default_Health;

            _timer = new Timer(GlobalEntityData.Plant_MungBeanShooter_ShootInterval);
            _detectTimer = new Timer(Global.Plant_Peashooter_DetectInterval);
            _layerMask = LayerMask.GetMask("Zombie", "Barrier");
        }

        [SerializeField] private Timer _timer;
        [SerializeField] private Timer _detectTimer;
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
                var hit = Physics2D.Raycast(FirePoint.position, Direction.ToVector2(),
                    GlobalEntityData.Plant_Peashooter_ShootDistance, _layerMask);

                if (hit.collider && hit.collider.CompareTag("Zombie"))
                {
                    EntityFactory.ProjectileFactory.CreatePea(ProjectileId.MungBean, Direction, FirePoint.position);
                    _timer.Reset();
                }
                else
                {
                    // 防止_detectTimer检测速度跟不上_timer导致的卡弹，所以_detectTimer放在这里重置
                    _detectTimer.Reset();
                }
            }
        }
    }
}