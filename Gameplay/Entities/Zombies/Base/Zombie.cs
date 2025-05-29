using System;
using DG.Tweening;
using QFramework;
using TPL.PVZR.Architecture.Managers;
using TPL.PVZR.Gameplay.Class;
using TPL.PVZR.Gameplay.Data;
using TPL.PVZR.Gameplay.Entities.Classes.Effects;
using TPL.PVZR.Gameplay.Entities.Classes.Effects.Interfaces;
using TPL.PVZR.Gameplay.Entities.Plants.Base;
using TPL.PVZR.Gameplay.Entities.Zombies.ViewControllers;
using TPL.PVZR.Gameplay.ViewControllers.InLevel;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace TPL.PVZR.Gameplay.Entities.Zombies.Base
{
    public interface IZombie : IEntity, IAttackable, IEffectable
    {
    }

    public abstract class Zombie : Entity, IZombie
    {
        #region Enum

        protected enum ZombieState
        {
            Idle,
            Targeting,
            Dead,
            Attacking
        }

        #endregion

        #region Behavior

        #region Effects

        protected EffectGroup _effectGroup = new();

        /// <summary>
        /// 需确保已在Update处驱动
        /// </summary>
        protected void EffectUpdate()
        {
            _effectGroup.ReduceDuration();
            while (_effectGroup.effectsToRecycle.TryPop(out var effect))
            {
                WhenEffectEnd(effect);
            }
        }
        public void GiveEffect(IEffect effect)
        {
            _effectGroup.Combine(effect, out var startEffect);
            if (startEffect)
            {
                WhenEffectStart(effect);
            }
        }

        protected void WhenEffectStart(IEffect effect)
        {
            switch (effect.effectId)
            {
                case EffectId.SnowSlowness:
                    moveSpeed *= 0.5f;
                    attack.SetDamage(attack.damage * 0.5f);
                    break;
                case EffectId.Buttered:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        // 以后肯定要改，性能有点小难绷
        protected void WhenEffectEnd(IEffect effect)
        {
            switch (effect.effectId)
            {
                case EffectId.SnowSlowness:
                    moveSpeed *= 2f;
                    attack.SetDamage(attack.damage * 2f);
                    break;
                case EffectId.Buttered:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        #endregion

        #region 变量

        // 待配置数据
        [SerializeField] protected float _initialHealthPoint_ = 100f;
        [SerializeField] protected float _defaultMoveSpeed_ = 1.5f;

        [SerializeField] protected AttackDataSO _attackDataSO_;

        // 属性
        protected float moveSpeed;
        protected BindableProperty<float> healthPoint;
        protected Attack attack;

        // 变量
        protected FSM<ZombieState> behaviorState = new();
        protected IAttackable attackingTarget = null;

        #endregion

        # region 一层具象(可调用方法)

        public virtual void TakeDamage(Attack attack)
        {
            if (attack is null) throw new ArgumentNullException("attack是null");
            if (attack.punchForce != 0)
            {
                _Rigidbody2D.DOMoveX(_Rigidbody2D
                    .position.x + attack.punchDirection.x * attack.punchForce, 0.1f);
            }

            if (attack.slowness)
            {
                this.GiveEffect(new GeneralEffect(EffectId.SnowSlowness,4f));
            }

            if (attack.butter)
            {
                this.GiveEffect(new GeneralEffect(EffectId.Buttered,4f));
            }

            // 血量减少放在后面，因为僵尸死亡时会导致DOTween被销毁导致报错。添加死亡阶段后就能解决这个问题
            if (attack.damage != 0)
            {
                this.healthPoint.Value -= attack.damage;
            }
        }

        protected virtual void Dead()
        {
            gameObject.DestroySelf();
            _EntitySystem.RemoveZombie(this);
        }

        protected virtual void Jump()
        {
            _Rigidbody2D.velocity = new Vector2(_Rigidbody2D.velocity.x, Global.zombieJumpSpeed);
        }

        public override void Kill()
        {
            healthPoint.Value = 0;
        }

        # endregion

        # region MainLogic

        protected virtual void SetUpState()
        {
            _ZombieAttackArea.OnTriggerEnterEvent += AITargeting_OnFindPlant;
            behaviorState ??= new FSM<ZombieState>();
            // Targeting
            // AITargeting寻找目标(然后向目标移动)，碰到植物就吃
            behaviorState.State(ZombieState.Targeting)
                .OnEnter(() => { })
                .OnUpdate(() => AITargeting_Target())
                .OnExit(() => { _ZombieAttackArea.OnTriggerEnterEvent -= AITargeting_OnFindPlant; })
                ;
            // Attacking
            behaviorState.State(ZombieState.Attacking)
                .OnEnter(() =>
                {
                    this._Rigidbody2D.velocity = new Vector2(0, this._Rigidbody2D.velocity.y);
                    _ZombieAttackArea.OnTriggerExitEvent += AIAttacking_OnLoseTarget;
                }).OnUpdate(() => { attackingTarget?.TakeDamage(attack); })
                .OnExit(() => { _ZombieAttackArea.OnTriggerExitEvent -= AIAttacking_OnLoseTarget; });

            // Dead
            behaviorState.State(ZombieState.Dead)
                .OnEnter(Dead);
            behaviorState.StartState(ZombieState.Targeting);
        }

        protected virtual void AITargeting_OnFindPlant(Collider2D other)
            // 检测植物, 如果检测到了就吃
        {
            if (other.CompareTag("Plant"))
            {
                this.behaviorState.ChangeState(ZombieState.Attacking);
                attackingTarget = other.GetComponent<Plant>();
            }
            else if (other.CompareTag("Dave"))
            {
                this.behaviorState.ChangeState(ZombieState.Attacking);
                attackingTarget = other.GetComponent<Dave>();
            }
        }

        protected virtual void AITargeting_Target()
            // 寻路中(跟随戴夫)
        {
            // 走路
            if (ReferenceModel.Get.Dave.transform.position.x > transform.position.x)
            {
                transform.LocalScaleX(-1);
            }
            else
            {
                transform.LocalScaleX(1);
            }

            _Rigidbody2D.velocity = new Vector2(-moveSpeed * transform.localScale.x, _Rigidbody2D.velocity.y);
            // 跳跃
            Collider2D col = Physics2D.Raycast(transform.position, new Vector2(-transform.localScale.x, 0), 0.9f,
                LayerMask.GetMask("Barrier")).collider;
            if (col)
            {
                Jump();
            }
        }

        protected virtual void AIAttacking_OnLoseTarget(Collider2D other)
            // 检测到攻击对象丢失
        {
            if (ReferenceEquals(other.gameObject, attackingTarget.gameObject))
            {
                // 满足条件时，原本锁定的对象丢失
                this.behaviorState.ChangeState(ZombieState.Targeting);
                attackingTarget = null;
            }
        }

        protected virtual void Update()
        {
            // Slowness
            EffectUpdate();
            //
            behaviorState.Update();
        }

        # endregion

        #endregion

        // 引用
        protected Rigidbody2D _Rigidbody2D;
        protected ZombieAttackArea _ZombieAttackArea;

        // 初始化
        protected override void OnAwakeBase()
        {
            _Rigidbody2D = GetComponent<Rigidbody2D>();
            _ZombieAttackArea = GetComponentInChildren<ZombieAttackArea>();
            // 初始化数据
            moveSpeed = _defaultMoveSpeed_;
            healthPoint = new BindableProperty<float>(_initialHealthPoint_);
            healthPoint.Register(val =>
            {
                if (val <= 0)
                {
                    behaviorState.ChangeState(ZombieState.Dead);
                }
            });
            attack = new Attack(_attackDataSO_);
            SetUpState();
        }
    }
}