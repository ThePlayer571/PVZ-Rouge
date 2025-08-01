using DG.Tweening;
using QFramework;
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
    public sealed partial class KernelPult : Plant
    {
        public override PlantDef Def { get; } = new PlantDef(PlantId.KernelPult, PlantVariant.V0);

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
                    OnThrow.Trigger();
                    if (RandomHelper.Default.NextBool(GlobalEntityData.Plant_KernelPult_ButterChance))
                    {
                        EntityFactory.ProjectileFactory.CreatePea(ProjectileId.Butter, _direction, FirePoint.position);
                    }
                    else
                    {
                        EntityFactory.ProjectileFactory.CreatePea(ProjectileId.Kernel, _direction, FirePoint.position);
                    }
                }
            }
        }
    }

    public partial class KernelPult
    {
        private EasyEvent OnThrow = new();
        [SerializeField] private Transform Thrower;


        private static readonly Vector3 DownRotation = new Vector3(0, 0, 2.219f);
        private static readonly Vector3 UpRotation = new Vector3(0, 0, -57.922f);

        protected override void OnViewInit()
        {
            OnThrow.Register(() =>
            {
                Thrower.DOLocalRotate(UpRotation, 0.1f).SetEase(Ease.OutQuint).OnComplete(() =>
                {
                    ActionKit.Delay(0.5f, () => Thrower.DOLocalRotate(DownRotation, 0.2f)).Start(this);
                });
            }).UnRegisterWhenGameObjectDestroyed(this);
        }
    }
}