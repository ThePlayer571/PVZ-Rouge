using DG.Tweening;
using QFramework;
using TPL.PVZR.Architecture;
using TPL.PVZR.Gameplay.Class;
using TPL.PVZR.Gameplay.Data;
using TPL.PVZR.Gameplay.Entities;
using UnityEngine;
using UnityEngine.Serialization;

namespace TPL.PVZR.Gameplay.ViewControllers.ZombieArmor.Base
{
    public enum ArmorState
    {
        Intact, // 完好无损
        Damaged, // 轻微受损
        Destroyed // 重度受损
    }

    public interface IArmor : IController, IDamageable
    {
        
    }

    public abstract class Armor : ViewController, IArmor
    {
        #region SerializeField

        

        [FormerlySerializedAs("zombieArmorData")]
        [SerializeField] public ArmorData armorData;
        #endregion

        // 属性
        protected float duration;

        // 变量
        protected FSM<ArmorState> armorState = new FSM<ArmorState>();
        

        // 引用
        protected SpriteRenderer spriteRenderer;
        protected Attack laseReceivedAttack = null;

        // 事件
        public EasyEvent OnArmorDestroyed = new();

        // StateMachine
        protected virtual void SetUpState()
        {
            armorState ??= new FSM<ArmorState>();
            armorState.State(ArmorState.Intact);
            armorState.State(ArmorState.Damaged);
            armorState.State(ArmorState.Destroyed)
                .OnEnter(() =>
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
                });
            //
            armorState.StartState(ArmorState.Intact);
        }

        // 初始化
        protected virtual void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            duration = armorData.duration;
            //
            SetUpState();
        }

        #region 函数
        protected virtual void ChangeStateByDuration()
        {
            if (armorState.CurrentStateId != ArmorState.Intact &&
                duration > armorData.damagedDuration)
            {
                armorState.ChangeState(ArmorState.Intact);
            }
            else if (armorState.CurrentStateId != ArmorState.Damaged &&
                     (duration <= armorData.damagedDuration && duration > 0))
            {
                armorState.ChangeState(ArmorState.Damaged);

            }
            else if (armorState.CurrentStateId != ArmorState.Destroyed && duration <= 0)
            {
                armorState.ChangeState(ArmorState.Destroyed);
            }
        }
        
        #endregion

        #region IArmor

        public void TakeDamage(Attack attack)
        {
            this.TakeDamage(attack,out var leftAttack);
        }

        public virtual void TakeDamage(Attack attack,out Attack leftAttack)
        {
            // 损伤耐久
            duration -= attack.damage;
            if (duration >= 0)
            {
                leftAttack = attack.DamageMultiplier(0);
            }
            else
            {
                leftAttack = attack.WithDamage(-duration);
            }
            // 处理行为状态
            ChangeStateByDuration();
        }
        public void Kill()
        {
            throw new System.NotImplementedException();
        }

        

        #endregion
        public IArchitecture GetArchitecture()
        {
            return PVZRouge.Interface;
        }

    }
}