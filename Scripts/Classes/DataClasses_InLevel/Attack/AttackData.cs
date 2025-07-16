using System;
using System.Collections.Generic;
using System.Linq;
using QFramework;
using TPL.PVZR.Classes.DataClasses.Effect;
using UnityEngine;

namespace TPL.PVZR.Classes.DataClasses.Attack
{
    public class AttackData
    {
        private float damage;
        private bool isFrameDamage;
        
        private float punchForce;
        private PunchType punchType;
        private Vector2 punchFrom;
        
        private List<EffectData> effects;


        #region 初始化函数

        public AttackData MultiplyDamage(float factor)
        {
            damage *= factor;
            return this;
        }

        public AttackData SubDamage(float value)
        {
            damage -= value;
            return this;
        }

        public AttackData MultiplyPunchForce(float factor)
        {
            punchForce *= factor;
            return this;
        }

        public AttackData OnlyPunch()
        {
            damage = 0;
            effects = new List<EffectData>();
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
            Vector2 direction;
            switch (punchType)
            {
                case PunchType.Free:
                    direction = (punchTo - punchFrom).normalized;
                    break;
                case PunchType.ConstrainHorizontal:
                    direction = (punchTo - punchFrom).x > 0 ? new Vector2(1, 0) : new Vector2(-1, 0);
                    break;
                default: throw new ArgumentException();
            }

            return direction * punchForce;
        }

        public List<EffectData> Effects => effects;

        #endregion

        #region 构造函数

        public AttackData(AttackData other)
        {
            this.damage = other.damage;
            this.punchForce = other.punchForce;
            this.punchType = other.punchType;
            this.isFrameDamage = other.isFrameDamage;
            this.effects = other.effects.Select(data => new EffectData(data)).ToList();
        }

        public AttackData(float damage, float punchForce, bool isFrameDamage)
        {
            throw new ArgumentException();
        }

        public AttackData(AttackDefinition attackDefinition)
        {
            this.damage = attackDefinition.damage;
            this.punchForce = attackDefinition.punchForce;
            this.punchType = attackDefinition.punchType;
            this.isFrameDamage = attackDefinition.isFrameDamage;
            this.effects = attackDefinition.effects.Select(definition => new EffectData(definition)).ToList();
        }

        #endregion
    }
}