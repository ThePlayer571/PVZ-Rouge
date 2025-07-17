using QFramework;
using TPL.PVZR.Classes.DataClasses_InLevel.Attack;
using TPL.PVZR.ViewControllers.Entities.EntityBase.Interfaces;
using UnityEngine;

namespace TPL.PVZR.Classes.DataClasses_InLevel.ZombieArmor
{
    public class ZombieArmorData : IAttackable
    {
        #region 数值

        private float ArmorInitialHealth { get; }
        private float DamageReductionRate { get; }
        private float PunchResistanceRate { get; }

        private float IntactMinRatio { get; }
        private float DamagedMinRatio { get; }
        private float BrokenMinRatio { get; }

        #endregion

        #region 变量

        private float armorHealth;
        private ArmorState armorState;
        public bool IsDestroyed { get; private set; }

        #endregion

        public readonly EasyEvent<ArmorState, AttackData> OnChangeState = new EasyEvent<ArmorState, AttackData>();
        public readonly EasyEvent OnDestroyed = new EasyEvent();

        public ZombieArmorData(ZombieArmorDefinition definition)
        {
            this.ArmorInitialHealth = definition.armorHealth;
            this.DamageReductionRate = definition.damageReductionRate;
            this.PunchResistanceRate = definition.punchResistanceRate;
            this.IntactMinRatio = definition.intactMinRatio;
            this.DamagedMinRatio = definition.damagedMinRatio;
            this.BrokenMinRatio = definition.brokenMinRatio;

            armorHealth = ArmorInitialHealth;
            UpdateArmorStateWithAttack(null);
        }

        private void UpdateArmorStateWithAttack(AttackData attackData)
        {
            ArmorState newState;

            // 计算新状态
            if (armorHealth >= ArmorInitialHealth * IntactMinRatio)
                newState = ArmorState.Intact;
            else if (armorHealth >= ArmorInitialHealth * DamagedMinRatio)
                newState = ArmorState.Damaged;
            else if (armorHealth > ArmorInitialHealth * BrokenMinRatio)
                newState = ArmorState.Broken;
            else
                newState = ArmorState.Destroyed;

            // 只在状态真正改变时触发事件
            if (armorState != newState)
            {
                armorState = newState;
                if (attackData != null)
                    OnChangeState.Trigger(newState, attackData);
            }
        }

        public AttackData TakeAttack(AttackData attackData)
        {
            // 计算伤害
            var damageToArmor = Mathf.Clamp(attackData.Damage * DamageReductionRate, 0, armorHealth);

            armorHealth -= damageToArmor;

            // 更新ArmorState
            UpdateArmorStateWithAttack(attackData);
            if (armorHealth <= 0)
            {
                IsDestroyed = true;
                OnDestroyed.Trigger();
            }

            attackData.SubDamage(damageToArmor);
            attackData.MultiplyPunchForce(1 - PunchResistanceRate);
            return attackData;
        }
    }
}