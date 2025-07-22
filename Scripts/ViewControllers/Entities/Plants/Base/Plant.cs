using System;
using QFramework;
using TPL.PVZR.Classes;
using TPL.PVZR.Classes.DataClasses_InLevel;
using TPL.PVZR.Classes.DataClasses_InLevel.Attack;
using TPL.PVZR.Classes.DataClasses;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.CommandEvents.Level_Gameplay.PlantSpawn;
using TPL.PVZR.Helpers.New.DataReader;
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
        public abstract PlantDef Def { get; }

        // 变量
        public Direction2 Direction { get; protected set; }
        public float HealthPoint { get; protected set; }

        #endregion


        #region 被攻击

        public virtual AttackData TakeAttack(AttackData attackData)
        {
            HealthPoint = Mathf.Clamp(HealthPoint - attackData.Damage, 0, Mathf.Infinity);
            if (HealthPoint <= 0) DieWith(attackData);
            return null;
        }

        #endregion

        #region 实体生命周期

        public override void DieWith(AttackData attackData)
        {
            this.SendCommand<RemovePlantCommand>(new RemovePlantCommand(this));
        }

        public override void Remove()
        {
            throw new NotSupportedException($"植物的移除操作应该通过命令来处理");
        }

        public void Initialize(Direction2 direction)
        {
            this.Direction = direction;
            gameObject.LocalScaleX(direction.ToInt());

            OnInit();
            OnViewInit();
        }

        protected override void Update()
        {
            OnUpdate();
            OnViewUpdate();
        }

        protected abstract void OnInit();

        protected virtual void OnUpdate()
        {
        }

        protected virtual void OnViewInit()
        {
        }

        protected virtual void OnViewUpdate()
        {
        }

        public virtual void OnRemoved()
        {
        }

        #endregion
    }
}