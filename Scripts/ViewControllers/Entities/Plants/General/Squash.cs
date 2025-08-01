using DG.Tweening;
using QFramework;
using TPL.PVZR.Classes.DataClasses_InLevel.Attack;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.Helpers.New;
using TPL.PVZR.Helpers.New.ClassCreator;
using TPL.PVZR.Tools;
using TPL.PVZR.ViewControllers.Entities.EntityBase.Interfaces;
using TPL.PVZR.ViewControllers.Entities.Plants.Base;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Entities.Plants
{
    public sealed class Squash : Plant
    {
        public override PlantDef Def { get; } = new PlantDef(PlantId.Squash, PlantVariant.V0);


        [SerializeField] private TriggerDetector ZombieDetector;

        private bool _attacked = false;

        protected override void OnInit()
        {
            ZombieDetector.RecordTargets = true;
            
            ZombieDetector.OnTargetEnter.Register(collider =>
            {
                if (_attacked) return;
                _attacked = true;

                var target = collider.gameObject;
                this.Direction = target.transform.position.x > transform.position.x
                    ? Direction2.Right
                    : Direction2.Left;

                ActionKit.Sequence()
                    .Callback(() => NoticeHelper.Notice("en?"))
                    .Delay(0.5f)
                    .Callback(() =>
                    {
                        var endPos = target != null
                            ? target.transform.position
                            : ZombieDetector.DetectedTargets.MinBy(collider2D =>
                                Vector2.Distance(collider2D.transform.position, transform.position)).transform.position;
                        _Rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
                        gameObject.layer = LayerMask.NameToLayer("Default");


                        _Rigidbody2D.DOMove(endPos + new Vector3(0, 1.2f), 0.5f).SetEase(Ease.OutExpo)
                            .OnComplete(() =>
                            {
                                _Rigidbody2D.DOMove(endPos, 0.1f)
                                    .OnComplete(() =>
                                    {
                                        var targets = Physics2D.OverlapCircleAll(_Rigidbody2D.position,
                                            GlobalEntityData.Plant_Squash_Radius, LayerMask.GetMask("Zombie"));
                                        foreach (var target in targets)
                                        {
                                            var attackData = AttackCreator.CreateAttackData(AttackId.Squash).WithPunchFrom(this.transform.position);
                                            target.GetComponent<IAttackable>().TakeAttack(attackData);
                                        }

                                        ActionKit.Delay(1f, Kill).Start(this);
                                    });
                            });
                    }).Start(this);
            }).UnRegisterWhenGameObjectDestroyed(this);
        }

        public override AttackData TakeAttack(AttackData attackData)
        {
            return null;
        }
    }
}