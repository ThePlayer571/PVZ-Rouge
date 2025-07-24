using QFramework;
using TPL.PVZR.Classes;
using TPL.PVZR.Classes.DataClasses_InLevel;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.Helpers;
using TPL.PVZR.Helpers.New.GameObjectFactory;
using TPL.PVZR.Helpers.New.Methods;
using TPL.PVZR.Models;
using TPL.PVZR.Tools;
using TPL.PVZR.ViewControllers.Entities.Plants.Base;
using UnityEngine;
using Time = UnityEngine.Time;

namespace TPL.PVZR.ViewControllers.Entities.Plants
{
    public sealed class Threepeater : Plant
    {
        public override PlantDef Def { get; } = new PlantDef(PlantId.Threepeater, PlantVariant.V0);

        protected override void OnInit()
        {
            this.HealthPoint = GlobalEntityData.Plant_Default_Health;

            _timer = new Timer(GlobalEntityData.Plant_Peashooter_ShootInterval);
            _detectTimer = new Timer(Global.Plant_Peashooter_DetectInterval);
            _layerMask = LayerMask.GetMask("Zombie", "Barrier");

            _firePoints = new[] { FirePoint_1, FirePoint_2, FirePoint_3 };

            _levelGridModel = this.GetModel<ILevelGridModel>();
        }

        private Timer _timer;
        private Timer _detectTimer;
        private int _layerMask;
        private ILevelGridModel _levelGridModel;

        [SerializeField] private Transform FirePoint_1;
        [SerializeField] private Transform FirePoint_2;
        [SerializeField] private Transform FirePoint_3;
        private Transform[] _firePoints;

        protected override void OnUpdate()
        {
            _timer.Update(Time.deltaTime);
            _detectTimer.Update(Time.deltaTime);

            if (_timer.Ready && _detectTimer.Ready)
            {
                _detectTimer.Reset();

                foreach (var firePoint in _firePoints)
                {
                    var hit = Physics2D.Raycast(
                        firePoint.position,
                        Direction.ToVector2(),
                        GlobalEntityData.Plant_Peashooter_ShootDistance,
                        _layerMask
                    );

                    if (hit.collider && hit.collider.CompareTag("Zombie"))
                    {
                        foreach (var fp in _firePoints)
                        {
                            var cell = _levelGridModel.GetCell(fp.position);
                            if (cell.Is(CellTypeId.Block)) continue;
                            EntityFactory.ProjectileFactory.CreatePea(ProjectileId.Pea, Direction.ToVector2(),
                                fp.position);
                        }

                        _timer.Reset();
                        break;
                    }
                }
            }
        }
    }
}