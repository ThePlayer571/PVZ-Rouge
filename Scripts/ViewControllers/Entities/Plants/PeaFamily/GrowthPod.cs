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
    public partial class GrowthPod
    {
        [SerializeField] private SpriteRenderer _SpriteRenderer;
        [SerializeField] private Sprite Level_1;
        [SerializeField] private Sprite Level_2;
        [SerializeField] private Sprite Level_3;
        [SerializeField] private Sprite Level_4;
        [SerializeField] private Sprite Level_5;

        protected override void OnViewInit()
        {
            _level.Register(level =>
            {
                switch (level)
                {
                    case 1:
                        _SpriteRenderer.sprite = Level_1;
                        break;
                    case 2:
                        _SpriteRenderer.sprite = Level_2;
                        break;
                    case 3:
                        _SpriteRenderer.sprite = Level_3;
                        break;
                    case 4:
                        _SpriteRenderer.sprite = Level_4;
                        break;
                    case 5:
                        _SpriteRenderer.sprite = Level_5;
                        break;
                }
            }).UnRegisterWhenGameObjectDestroyed(this);
        }
    }

    public sealed partial class GrowthPod : Plant, ICanBeStackedOn
    {
        public override PlantDef Def { get; } = new PlantDef(PlantId.PeaPod, PlantVariant.V1);


        public bool CanStack(PlantDef plantDef)
        {
            return _level.Value < 5 && plantDef == Def;
        }

        public void StackAdd()
        {
            _level.Value++;
        }

        protected override void OnInit()
        {
            this.HealthPoint = GlobalEntityData.Plant_Default_Health;

            _timer = new Timer(GlobalEntityData.Plant_Peashooter_ShootInterval);
            _detectTimer = new Timer(Global.Plant_Peashooter_DetectInterval);
            _growthTimer = new Timer(GlobalEntityData.Plant_GrowthPod_GrowTime);
            _layerMask = LayerMask.GetMask("Zombie", "Barrier");

            _firePoints = new[] { FirePoint_1, FirePoint_2, FirePoint_3, FirePoint_4, FirePoint_5 };
        }

        private Timer _timer;
        private Timer _growthTimer;
        private Timer _detectTimer;
        private int _layerMask;

        private BindableProperty<int> _level = new(1);

        [SerializeField] private Transform FirePoint_1;
        [SerializeField] private Transform FirePoint_2;
        [SerializeField] private Transform FirePoint_3;
        [SerializeField] private Transform FirePoint_4;
        [SerializeField] private Transform FirePoint_5;
        private Transform[] _firePoints;


        protected override void OnUpdate()
        {
            // 发射
            _timer.Update(Time.deltaTime);
            _detectTimer.Update(Time.deltaTime);

            if (_timer.Ready && _detectTimer.Ready)
            {
                _detectTimer.Reset();
                var hit = Physics2D.Raycast(FirePoint_4.position, Direction.ToVector2(),
                    GlobalEntityData.Plant_Peashooter_ShootDistance, _layerMask);

                if (hit.collider && hit.collider.CompareTag("Zombie"))
                {
                    _timer.Reset();
                    for (int i = 0; i < _level.Value; i++)
                    {
                        _ProjectileService.CreatePea(ProjectileId.Pea, Direction.ToVector2(),
                            _firePoints[i].position);
                    }
                }
            }
            // 生长
            _growthTimer.Update(Time.deltaTime);

            if (_growthTimer.Ready)
            {
                if (CanStack(this.Def))
                {
                    StackAdd();
                }
            }
        }
    }
}