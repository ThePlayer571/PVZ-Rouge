using DG.Tweening;
using QFramework;
using TPL.PVZR.Classes.DataClasses_InLevel;
using TPL.PVZR.Classes.DataClasses_InLevel.Attack;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.CommandEvents.Level_Shit;
using TPL.PVZR.Helpers.New.ClassCreator;
using TPL.PVZR.Models;
using TPL.PVZR.Tools;
using TPL.PVZR.ViewControllers.Entities.EntityBase.Interfaces;
using TPL.PVZR.ViewControllers.Entities.Plants.Base;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Entities.Plants
{
    public sealed class GraveBuster : Plant
    {
        public override PlantDef Def { get; } = new PlantDef(PlantId.GraveBuster, PlantVariant.V0);

        protected override void OnInit()
        {
            transform.DOScale(transform.localScale * 0.5f, 3f).OnComplete(() =>
            {
                var _LevelGridModel = this.GetModel<ILevelGridModel>();
                var cellPos = this.CellPos;
                this.SendCommand<BreakGravestoneCommand>(new BreakGravestoneCommand(cellPos));

                Kill();
            });
        }
    }
}