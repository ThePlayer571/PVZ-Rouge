using System;
using System.Collections.Generic;
using System.Linq;
using QFramework;
using TPL.PVZR.Classes.DataClasses_InLevel.Effect;
using UnityEngine;

namespace TPL.PVZR.Classes.DataClasses_InLevel.Attack
{
    public class AttackData
    {
        private float damage;
        private bool isFrameDamage;

        private float punchForce;
        private PunchType punchType;
        private Vector2? punchFrom;
        private Vector2? punchDirection;

        private List<EffectData> effects;


        #region 修改数据的方法

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

        public AttackData WithPunchDirection(Vector2 direction)
        {
            punchDirection = direction.normalized;
            return this;
        }

        public AttackData WithPunchDirectionX(float direction)
        {
            punchDirection = new Vector2(direction.Sign(), 0);
            return this;
        }

        #endregion

        #region 读取数据的方法

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
            if (punchForce == 0) return Vector2.zero;
            // 
            var direction = GetDirection(punchTo);
            return direction * punchForce;
        }

        public List<EffectData> Effects => effects;

        private Vector2 GetDirection(Vector2 punchTo)
        {
            return punchType switch
            {
                PunchType.ByRelativePosition => (punchTo - punchFrom.Value).normalized,
                PunchType.ConstrainHorizontal => new Vector2((punchTo.x - punchFrom.Value.x).Sign(), 0),
                PunchType.ConstrainUp => Vector2.up,
                PunchType.ConstrainDown => Vector2.down,
                PunchType.ByPresetDirection => punchDirection.Value,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        #endregion

        #region 构造函数

        public AttackData(AttackData other)
        {
            this.damage = other.damage;
            this.punchForce = other.punchForce;
            this.punchType = other.punchType;
            this.punchFrom = other.punchFrom;
            this.punchDirection = other.punchDirection;
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