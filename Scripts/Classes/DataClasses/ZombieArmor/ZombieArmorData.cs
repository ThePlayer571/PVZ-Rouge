using QFramework;
using TPL.PVZR.Classes.DataClasses.Attack;
using TPL.PVZR.ViewControllers.Entities.EntityBase.Interfaces;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Entities.Zombies.ZombieArmor
{
    public class ZombieArmorData : IAttackable
    {
        public readonly float armorInitialHealth;
        public readonly float damageReductionRate;
        public readonly float punchResistanceRate;

        public readonly float intactMinRatio;
        public readonly float damagedMinRatio;
        public readonly float brokenMinRatio;

        public float armorHealth;
        public ArmorState armorState;

        public EasyEvent<ArmorState, AttackData> OnChangeState = new EasyEvent<ArmorState, AttackData>();

        public ZombieArmorData(ZombieArmorDefinition definition)
        {
            this.armorInitialHealth = definition.armorHealth;
            this.damageReductionRate = definition.damageReductionRate;
            this.punchResistanceRate = definition.punchResistanceRate;
            this.intactMinRatio = definition.intactMinRatio;
            this.damagedMinRatio = definition.damagedMinRatio;
            this.brokenMinRatio = definition.brokenMinRatio;

            armorHealth = armorInitialHealth;
            UpdateArmorStateWithAttack(null);
        }

        private void UpdateArmorStateWithAttack(AttackData attackData)
        {
            ArmorState newState;

            // 计算新状态
            if (armorHealth >= armorInitialHealth * intactMinRatio)
                newState = ArmorState.Intact;
            else if (armorHealth >= armorInitialHealth * damagedMinRatio)
                newState = ArmorState.Damaged;
            else if (armorHealth > armorInitialHealth * brokenMinRatio)
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
            var damageToArmor = Mathf.Clamp(attackData.Damage * damageReductionRate, 0, armorHealth);

            armorHealth -= damageToArmor;

            // 更新ArmorState
            UpdateArmorStateWithAttack(attackData);

            attackData.SubDamage(damageToArmor);
            attackData.MultiplyPunchForce(1 - punchResistanceRate);
            return attackData;
        }
    }
}