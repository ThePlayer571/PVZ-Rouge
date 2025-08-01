using TPL.PVZR.Classes.DataClasses_InLevel.Attack;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.Helpers.New.ClassCreator;
using TPL.PVZR.Tools;
using TPL.PVZR.ViewControllers.Entities.EntityBase.Interfaces;
using TPL.PVZR.ViewControllers.Entities.Plants.Base;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Entities.Plants
{
    public sealed class BonkChoy : Plant
    {
        public override PlantDef Def { get; } = new PlantDef(PlantId.BonkChoy, PlantVariant.V0);

        protected override void OnInit()
        {
            this.HealthPoint = GlobalEntityData.Plant_Default_Health;

            _timer = new Timer(GlobalEntityData.Plant_BonkChoy_HitInterval);

            ZombieDetector.RecordTargets = true;
        }

        private Timer _timer;
        private int _hitCount = 0;

        [SerializeField] private TriggerDetector ZombieDetector;

        protected override void OnUpdate()
        {
            _timer.Update(Time.deltaTime);

            if (_timer.Ready && ZombieDetector.HasTarget)
            {
                _timer.Reset();
                _hitCount++;
                var zombie = ZombieDetector.DetectedTargets.MinBy( z => Vector2.Distance( 
                    z.transform.position, this.transform.position)).gameObject.GetComponent<IAttackable>();
                var attackData = _hitCount % 3 == 0
                    ? AttackCreator.CreateAttackData(AttackId.BonkChoyHeavy).WithPunchFrom(this.transform.position)
                    : AttackCreator.CreateAttackData(AttackId.BonkChoyLight).WithPunchFrom(this.transform.position);
                zombie.TakeAttack(attackData);
            }
        }
    }
}