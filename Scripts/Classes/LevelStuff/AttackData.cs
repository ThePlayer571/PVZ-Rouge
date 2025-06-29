using System;
using System.Collections.Generic;
using System.Linq;
using TPL.PVZR.Classes.LevelStuff.Effect;
using UnityEngine;

namespace TPL.PVZR.Classes.LevelStuff
{
    public class AttackData
    {
        private float damage;
        private float punchForce;
        private bool isFrameDamage;
        private List<EffectData> effects;
        // 我想或许不用让外界访问AttackDefinition，而是提供方便的方法调用

        private Vector2 punchFrom;

        #region 初始化函数

        public AttackData MultiplyDamage(float factor)
        {
            damage *= factor;
            return this;
        }

        public AttackData WithPunchFrom(Vector2 punchFrom)
        {
            this.punchFrom = punchFrom;
            return this;
        }

        #endregion

        #region 读取数据的函数

        public float Damage
        {
            get
            {
                if (isFrameDamage)
                {
                    return Time.deltaTime * damage;
                }
                else
                {
                    return damage;
                }
            }
        }

        public Vector2 Punch(Vector2 punchTo)
        {
            var direction = (punchTo - punchFrom).normalized;
            return direction * punchForce;
        }

        public List<EffectData> Effects => effects;

        #endregion

        #region 构造函数

        public AttackData(AttackData other)
        {
            this.damage = other.damage;
            this.punchForce = other.punchForce;
            this.isFrameDamage = other.isFrameDamage;
            this.effects = other.effects.ToList();
        }

        public AttackData(float damage, float punchForce, bool isFrameDamage)
        {
            throw new NotImplementedException();
            this.damage = damage;
            this.punchForce = punchForce;
            this.isFrameDamage = isFrameDamage;
            this.effects = new List<EffectData>();
        }

        public AttackData(AttackDefinition attackDefinition)
        {
            this.damage = attackDefinition.damage;
            this.punchForce = attackDefinition.punchForce;
            this.isFrameDamage = attackDefinition.isFrameDamage;
            this.effects = attackDefinition.effects.Select(definition => new EffectData(definition)).ToList();
        }

        #endregion
    }
}