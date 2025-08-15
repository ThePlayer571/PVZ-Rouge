using DG.Tweening;
using QFramework;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.Helpers.New.GameObjectFactory;
using TPL.PVZR.Helpers.New.Methods;
using TPL.PVZR.Tools;
using TPL.PVZR.ViewControllers.Entities.Plants.Base;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Entities.Plants
{
    public partial class DoubleCorePea
    {
        private bool _directionForward = true;
        private Timer _directionTimer;

        [SerializeField] private Transform Head;

        protected override void OnViewInit()
        {
            _directionTimer = new Timer(GlobalEntityData.Plant_Peashooter_ShootInterval / 2);
        }

        protected override void OnViewUpdate()
        {
            if (_directionForward == _forwardIsCloser)
            {
                _directionTimer.Update(Time.deltaTime);
                if (_directionTimer.Ready)
                {
                    // 换朝向
                    if (_forwardIsCloser)
                    {
                        Head.DORotate(new Vector3(0, 0, 180), 0.1f);
                    }
                    else
                    {
                        Head.DORotate(new Vector3(0, 0, 0), 0.1f);
                    }

                    //
                    _directionForward = !_forwardIsCloser;
                    _directionTimer.Reset();
                }
            }
        }
    }

    public sealed partial class DoubleCorePea : Plant
    {
        public override PlantDef Def { get; } = new PlantDef(PlantId.SplitPea, PlantVariant.V1);

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
        private bool _forwardIsCloser = false;

        protected override void OnUpdate()
        {
            _timer.Update(Time.deltaTime);
            _detectTimer.Update(Time.deltaTime);

            if (_timer.Ready && _detectTimer.Ready)
            {
                _detectTimer.Reset();
                // 存在特殊需求：在射击后设置朝向（符合表现的逻辑）
                // 检测朝向
                var hitForward = Physics2D.Raycast(FirePointForward.position, Direction.ToVector2(),
                    GlobalEntityData.Plant_Peashooter_ShootDistance, _layerMask);
                var hitBackward = Physics2D.Raycast(FirePointBackward.position, Direction.Reverse().ToVector2(),
                    GlobalEntityData.Plant_Peashooter_ShootDistance, _layerMask);
                var hasZombieForward = hitForward.collider && hitForward.collider.CompareTag("Zombie");
                var hasZombieBackward = hitForward.collider && hitBackward.collider.CompareTag("Zombie");

                bool temp_forwardIsCloser;
                if (hasZombieForward && hasZombieBackward)
                {
                    var distanceForward = Mathf.Abs(hitForward.point.x - FirePointForward.position.x);
                    var distanceBackward = Mathf.Abs(hitBackward.point.x - FirePointBackward.position.x);
                    temp_forwardIsCloser = distanceForward < distanceBackward;
                }
                else if (hasZombieForward)
                {
                    temp_forwardIsCloser = true;
                }
                else if (hasZombieBackward)
                {
                    temp_forwardIsCloser = false;
                }
                else
                {
                    temp_forwardIsCloser = _forwardIsCloser;
                }

                // 射击
                if (hasZombieBackward || hasZombieForward)
                {
                    // FirePoint选取
                    Vector2 closerFirePoint =
                        _forwardIsCloser ? FirePointForward.position : FirePointBackward.position;
                    Vector2 closerDirection =
                        _forwardIsCloser ? Direction.ToVector2() : Direction.Reverse().ToVector2();
                    Vector2 fartherFirePoint =
                        _forwardIsCloser ? FirePointBackward.position : FirePointForward.position;
                    Vector2 fartherDirection =
                        _forwardIsCloser ? Direction.Reverse().ToVector2() : Direction.ToVector2();

                    // farther
                    _ProjectileService.CreatePea(ProjectileId.Pea, fartherDirection, fartherFirePoint);
                    // closer
                    ActionKit.Sequence()
                        .Callback(() =>
                            _ProjectileService.CreatePea(ProjectileId.Pea, closerDirection, closerFirePoint))
                        .Delay(GlobalEntityData.Plant_Repeater_PeaInterval)
                        .Callback(() =>
                            _ProjectileService.CreatePea(ProjectileId.Pea, closerDirection, closerFirePoint))
                        .Delay(GlobalEntityData.Plant_Repeater_PeaInterval)
                        .Callback(() =>
                            _ProjectileService.CreatePea(ProjectileId.Pea, closerDirection, closerFirePoint))
                        .Start(this);

                    _timer.Reset();
                }

                // 调整朝向
                _forwardIsCloser = temp_forwardIsCloser;
            }
        }
    }
}