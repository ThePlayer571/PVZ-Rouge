using System.Linq.Expressions;
using QFramework;
using TPL.PVZR.Classes.DataClasses_InLevel.Attack;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.Helpers.New.ClassCreator;
using TPL.PVZR.Tools;
using TPL.PVZR.ViewControllers.Entities.EntityBase.Interfaces;
using TPL.PVZR.ViewControllers.Entities.Plants.Base;
using TPL.PVZR.ViewControllers.Entities.Zombies.Base;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Entities.Plants
{
    public partial class Chomper
    {
        protected override void OnViewInit()
        {
            TryDevourEvent.Register(() => { _Animator.SetTrigger("TryDevour"); })
                .UnRegisterWhenGameObjectDestroyed(this);
            _devourTimer.OnReadyChangeEvent.Register(ready => { _Animator.SetBool("IsChewing", !ready); })
                .UnRegisterWhenGameObjectDestroyed(this);
        }

        public void SetTriggerDevour()
        {
            _devourTrigger = true;
        }

        private bool _devourTrigger = false;

        private EasyEvent TryDevourEvent = new EasyEvent();
    }


    public sealed partial class Chomper : Plant
    {
        public override PlantDef Def { get; } = new PlantDef(PlantId.Chomper, PlantVariant.V0);

        [SerializeField] private TriggerDetector ZombieDetector;

        private Timer _devourTimer;

        protected override void OnUpdate()
        {
            _devourTimer.Update(Time.deltaTime);

            if (_devourTimer.Ready && ZombieDetector.HasTarget)
            {
                ActionKit.Sequence()
                    .Callback(() => { TryDevourEvent.Trigger(); })
                    .Condition(() => _devourTrigger)
                    .Callback(() =>
                    {
                        _devourTrigger = false;
                        Devour();
                    }).Start(this);
            }
        }

        protected override void OnInit()
        {
            HealthPoint = GlobalEntityData.Plant_Default_Health;
            ZombieDetector.RecordTargets = true;
            ZombieDetector.TargetPredicate = collider => collider.CompareTag("Zombie");

            _devourTimer = new Timer(GlobalEntityData.Plant_Chomper_ChewTime);
            _devourTimer.SetRemaining(0);
        }

        private void Devour()
        {
            if (!_devourTimer.Ready) return;

            if (ZombieDetector.HasTarget)
            {
                var targetZombie = ZombieDetector.DetectedTargets.MinBy(other =>
                    Vector2.Distance(this.transform.position, other.transform.position)).GetComponent<IAttackable>();
                targetZombie.TakeAttack(AttackCreator.CreateAttackData(AttackId.ChomperDevour));
                _devourTimer.Reset();
            }
            else
            {
                // do nothing
            }
        }
    }
}