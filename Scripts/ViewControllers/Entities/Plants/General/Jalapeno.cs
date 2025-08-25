using System.Collections.Generic;
using DG.Tweening;
using QFramework;
using TPL.PVZR.Classes.DataClasses_InLevel.Attack;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.CommandEvents.Level_Shit;
using TPL.PVZR.Helpers.New.ClassCreator;
using TPL.PVZR.Helpers.New.Methods;
using TPL.PVZR.Tools;
using TPL.PVZR.ViewControllers.Entities.EntityBase.Interfaces;
using TPL.PVZR.ViewControllers.Entities.Plants.Base;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Entities.Plants
{
    public sealed class Jalapeno : Plant
    {
        public override PlantDef Def { get; } = new PlantDef(PlantId.Jalapeno, PlantVariant.V0);


        [SerializeField] private Transform RaycastOrigin;

        protected override void OnInit()
        {
            transform.DOScale(transform.localScale * 1.5f, 0.3f).onComplete = () =>
            {
                var posLeft = Physics2DExtensions.GetRaycastEndPoint(RaycastOrigin.position, Vector2.left,
                    GlobalEntityData.Plant_Jalapeno_ExplosionLength, LayerMask.GetMask("Barrier"));
                var posRight = Physics2DExtensions.GetRaycastEndPoint(RaycastOrigin.position, Vector2.right,
                    GlobalEntityData.Plant_Jalapeno_ExplosionLength, LayerMask.GetMask("Barrier"));
                var targets = Physics2D.LinecastAll(posLeft, posRight, LayerMask.GetMask("Zombie"));
                var attackTemplate = AttackCreator.CreateAttackData(AttackId.JalapenoExplosion);

                foreach (var target in targets)
                {
                    var attackData = new AttackData(attackTemplate);
                    target.collider.GetComponent<IAttackable>().TakeAttack(attackData);
                }
                //

                this.SendCommand<ExplodeCommand>(new ExplodeCommand(
                    CellSelectHelper.GetCellsInRect(LevelGridHelper.WorldToCell(posLeft),
                            LevelGridHelper.WorldToCell(posRight)) as
                        IReadOnlyList<Vector2Int>, true));

                //
                Kill();
            };
        }

        public override AttackData TakeAttack(AttackData attackData)
        {
            return null;
        }
    }
}