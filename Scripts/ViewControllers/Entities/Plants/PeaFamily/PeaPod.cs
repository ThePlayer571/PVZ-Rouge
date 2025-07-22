using QFramework;
using TPL.PVZR.Classes;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.Helpers;
using TPL.PVZR.Helpers.New.GameObjectFactory;
using TPL.PVZR.Helpers.New.Methods;
using TPL.PVZR.Tools;
using TPL.PVZR.ViewControllers.Entities.Plants.Base;
using UnityEngine;
using Time = UnityEngine.Time;

namespace TPL.PVZR.ViewControllers.Entities.Plants
{
    public sealed class PeaPod : Plant, ICanBeStackedOn
    {
        public override PlantDef Def { get; } = new PlantDef(PlantId.PeaPod, PlantVariant.V0);


        public bool CanStack(PlantDef plantDef)
        {
            return _level < 5 && plantDef == Def;
        }

        public void StackAdd()
        {
            _level++;
        }

        protected override void OnInit()
        {
            this.HealthPoint = GlobalEntityData.Plant_Default_Health;

            _timer = new Timer(GlobalEntityData.Plant_Peashooter_ShootInterval);
            _detectTimer = new Timer(Global.Plant_Peashooter_DetectInterval);
            _layerMask = LayerMask.GetMask("Zombie", "Barrier");

            _firePoints = new[] { FirePoint_1, FirePoint_2, FirePoint_3, FirePoint_4, FirePoint_5 };
        }

        private Timer _timer;
        private Timer _detectTimer;
        private int _layerMask;

        private int _level = 1;

        [SerializeField] private Transform FirePoint_1;
        [SerializeField] private Transform FirePoint_2;
        [SerializeField] private Transform FirePoint_3;
        [SerializeField] private Transform FirePoint_4;
        [SerializeField] private Transform FirePoint_5;
        private Transform[] _firePoints;


        protected override void OnUpdate()
        {
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
                    for (int i = 0; i < _level; i++)
                    {
                        EntityFactory.ProjectileFactory.CreatePea(ProjectileId.Pea, Direction.ToVector2(),
                            _firePoints[i].position);
                    }
                }
            }
        }
    }
}