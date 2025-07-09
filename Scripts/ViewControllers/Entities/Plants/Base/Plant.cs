using QFramework;
using TPL.PVZR.Classes;
using TPL.PVZR.Classes.DataClasses;
using TPL.PVZR.Classes.DataClasses.Attack;
using TPL.PVZR.Models;
using TPL.PVZR.Tools;
using TPL.PVZR.ViewControllers.Entities.EntityBase;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Entities.Plants.Base
{
    public abstract class Plant : Entity, IPlant
    {
        #region 调试

        //

        #endregion

        #region 字段

        // 基础属性
        public abstract PlantId Id { get; }

        // 变量
        public Direction2 Direction { get; protected set; }
        public float HealthPoint { get; protected set; }

        #endregion

        public virtual void Initialize(Direction2 direction)
        {
            this.Direction = direction;

            gameObject.LocalScaleX(direction.ToInt());
        }

        #region 被攻击

        public AttackData TakeAttack(AttackData attackData)
        {
            HealthPoint = Mathf.Clamp(HealthPoint - attackData.Damage, 0, Mathf.Infinity);
            if (HealthPoint <= 0) DieWith(attackData);
            return null;
        }

        #endregion

        #region 实体生命周期

        public override void DieWith(AttackData attackData)
        {
            Remove();
        }

        public override void Remove()
        {
            var LevelGridModel = this.GetModel<ILevelGridModel>();
            var cell = LevelGridModel.GetCell(CellPos);
            cell.CellPlantState = CellPlantState.Empty;
            cell.Plant = null;

            base.Remove();
        }

        #endregion
    }
}