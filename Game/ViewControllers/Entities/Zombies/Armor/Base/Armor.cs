using System;
using DG.Tweening;
using QFramework;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace TPL.PVZR.EntityZombie
{
    public enum ArmorState
    {
        Intact,
        Damaged,
        Destroyed
    }

    public abstract class Armor : ViewController, IController, IDealAttack
    {
        // 可配置项
        [FormerlySerializedAs("zombieArmorData")]
        public ArmorData armorData;

        // 属性
        protected BindableProperty<float> duration;

        // 变量
        protected FSM<ArmorState> armorState = new FSM<ArmorState>();

        protected Attack lastReceivedAttack;

        // 引用
        protected SpriteRenderer spriteRenderer;

        // 事件
        public EasyEvent<Attack> OnArmorAttacked = new(); // para: 经盔甲抵消后的伤害
        public EasyEvent OnArmorDamaged = new();

        public EasyEvent OnArmorDestroyed = new();

        // StateMachine
        protected virtual void SetUpStateMachine()
        {
            armorState ??= new FSM<ArmorState>();
            armorState.State(ArmorState.Intact)
                .OnEnter(OnIntact);
            armorState.State(ArmorState.Damaged)
                .OnEnter(OnDamaged);
            armorState.State(ArmorState.Destroyed)
                .OnEnter(OnDestroyed);
            armorState.StartState(ArmorState.Intact);
        }

        protected virtual void OnIntact()
        {

        }

        protected virtual void OnDamaged()
        {

        }

        protected virtual void OnDestroyed()
        {
            transform.SetParent(null);
            OnArmorDestroyed.Trigger();
            ActionKit.Sequence()
                .Callback(() =>
                {
                    transform.DOJump(
                        transform.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(0, 0.3f)),
                        0.5f, 1, 0.5f);
                })
                .Delay(1).Callback(() => { spriteRenderer.DOFade(0, 0.5f); })
                .Delay(1).Callback(() => gameObject.DestroySelf())
                .Start(this);
        }

        // 初始化
        protected virtual void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            //
            duration ??= new BindableProperty<float>(armorData.duration);
            SetUpStateMachine();
        }

        // 函数
        protected virtual Attack GetAttackAfterArmor(Attack attack)
        {
            Attack attackAfterArmor = attack;
            // Damage
            attackAfterArmor.damage -= this.duration.Value;
            if (attackAfterArmor.damage < 0)
            {
                attackAfterArmor.damage = 0;
            }

            //
            return attackAfterArmor;
        }

        protected virtual void ChangeStateByDuration()
        {
            if (armorState.CurrentStateId != ArmorState.Intact &&
                duration.Value > armorData.damagedDuration)
            {
                armorState.ChangeState(ArmorState.Intact);

            }
            else if (armorState.CurrentStateId != ArmorState.Damaged &&
                     (duration.Value <= armorData.damagedDuration && duration.Value > 0))
            {
                armorState.ChangeState(ArmorState.Damaged);

            }
            else if (armorState.CurrentStateId != ArmorState.Destroyed && duration.Value <= 0)
            {
                armorState.ChangeState(ArmorState.Destroyed);
            }
        }

        // 不知道为什么，必须实现这个接口才行，不然会报错
        public virtual void Kill()
        {
            duration.Value = 0;
        }

        public virtual void DealAttack(Attack attack)
        {
            lastReceivedAttack = attack;
            OnArmorAttacked.Trigger(GetAttackAfterArmor(attack));
            duration.Value -= attack.damage;
            ChangeStateByDuration();
        }


        // 架构
        public IArchitecture GetArchitecture()
        {
            return PVZRouge.Interface;
        }

    }
}