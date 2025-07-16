using QFramework;
using TPL.PVZR.Classes;
using TPL.PVZR.Helpers;
using TPL.PVZR.Helpers.Methods;
using TPL.PVZR.Tools;
using TPL.PVZR.ViewControllers.Entities.Plants.Base;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Entities.Plants
{
    public sealed class SplitPea : Plant
    {
        public override PlantDef Def { get; } = new PlantDef(PlantId.SplitPea, PlantVariant.V0);

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

        [SerializeField] private Transform FirePointForward;
        [SerializeField] private Transform FirePointBackward;

        protected override void Update()
        {
            base.Update();
            //
            _timer.Update(Time.deltaTime);
            _detectTimer.Update(Time.deltaTime);

            if (_timer.Ready && _detectTimer.Ready)
            {
                _detectTimer.Reset();
                var pass = false;

                var hitForward = Physics2D.Raycast(FirePointForward.position, Direction.ToVector2(),
                    GlobalEntityData.Plant_Peashooter_ShootDistance, _layerMask);
                if (hitForward.collider && hitForward.collider.CompareTag("Zombie")) pass = true;

                if (!pass)
                {
                    var hitBackward = Physics2D.Raycast(FirePointBackward.position, Direction.Reverse().ToVector2(),
                        GlobalEntityData.Plant_Peashooter_ShootDistance, _layerMask);
                    if (hitBackward.collider && hitBackward.collider.CompareTag("Zombie")) pass = true;
                }

                if (pass)
                {
                    EntityFactory.ProjectileFactory.CreatePea(ProjectileId.Pea, Direction, FirePointForward.position);

                    ActionKit.Sequence()
                        .Callback(() =>
                            EntityFactory.ProjectileFactory.CreatePea(ProjectileId.Pea, Direction.Reverse(),
                                FirePointBackward.position))
                        .Delay(GlobalEntityData.Plant_Repeater_PeaInterval)
                        .Callback(() =>
                            EntityFactory.ProjectileFactory.CreatePea(ProjectileId.Pea, Direction.Reverse(),
                                FirePointBackward.position))
                        .Start(this);

                    _timer.Reset();
                }
            }
        }
    }
}