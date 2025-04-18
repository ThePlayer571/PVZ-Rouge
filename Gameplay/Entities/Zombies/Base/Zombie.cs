using System;
using DG.Tweening;
using QFramework;
using TPL.PVZR.Architecture.Managers;
using TPL.PVZR.Gameplay.Class;
using TPL.PVZR.Gameplay.Data;
using TPL.PVZR.Gameplay.Entities.Plants.Base;
using TPL.PVZR.Gameplay.ViewControllers.InLevel;
using UnityEngine;

namespace TPL.PVZR.Gameplay.Entities.Zombies.Base
{
    public interface IZombie : IEntity, IDamageable
    {
    }

    public abstract class Zombie : Entity, IZombie
    {
        #region 定义

        protected enum ZombieState
        {
            Idle,
            Targeting,
            Dead,
            Attacking
        }

        #endregion

        #region Behavior

        #region 声明

        // 属性
        protected BindableProperty<float> healthPoint;
        protected readonly Attack attack = new Attack(damage: 50, isFrameDamage: true);
        protected float moveSpeed = 1.5f;

        // 变量
        protected FSM<ZombieState> behaviorState = new();
        protected IDamageable attackingTarget = null;

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
                StartSlowness();
            }

            // 血量减少放在后面，因为僵尸死亡时会导致DOTween被销毁导致报错。添加死亡阶段后就能解决这个问题
            if (attack.damage != 0)
            {
                this.healthPoint.Value -= attack.damage;
            }
        }
        protected virtual void Dead()
        {
            DOTween.Kill(_Rigidbody2D);

            gameObject.DestroySelf();
            _EntitySystem.DestroyZombie(this);
        }

        protected virtual void Jump()
        {
            _Rigidbody2D.velocity = new Vector2(_Rigidbody2D.velocity.x, Global.zombieJumpSpeed);
        }

        public override void Kill()
        {
            healthPoint.Value = 0;
        }

        // slowness
        protected bool slowness = false;
        protected float slowTime;

        protected virtual void StartSlowness()
        {
            if (!slowness)
            {
                moveSpeed *= 0.5f;
                attack.SetDamage(attack.damage * 0.5f);
            }

            slowness = true;
            slowTime = 3f;
        }

        protected virtual void EndSlowness()
        {
            attack.SetDamage(attack.damage * 2f);
            moveSpeed *= 2f;
            slowness = false;
        }

        # endregion

        # region Logic

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
            if (slowness)
            {
                slowTime -= Time.deltaTime;
                if (slowTime <= 0)
                {
                    EndSlowness();
                }
            }

            //
            behaviorState.Update();
        }

        # endregion

        #endregion


        // 初始化
        public void Initialize()
        {
            // 初始化数据
            healthPoint = new BindableProperty<float>(114);
            healthPoint.Register(val =>
            {
                if (val <= 0)
                {
                    behaviorState.ChangeState(ZombieState.Dead);
                }
            });
            SetUpState();
        }


        // 引用
        protected Rigidbody2D _Rigidbody2D;

        protected ZombieAttackArea _ZombieAttackArea;

        // 初始化
        protected override void Awake()
        {
            base.Awake();
            //
            // 获取
            _Rigidbody2D = GetComponent<Rigidbody2D>();
            _ZombieAttackArea = GetComponentInChildren<ZombieAttackArea>();
        }
    }
}