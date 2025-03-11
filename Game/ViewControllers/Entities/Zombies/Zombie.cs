using System;
using DG.Tweening;
using UnityEngine;
using QFramework;
using TPL.PVZR.EntityPlant;
using Unity.VisualScripting;

namespace TPL.PVZR.EntityZombie
{
    public interface IZombie:IEntity,IDealAttack
    {
    }

    public enum ZombieState
    {
        Idle, Targeting, Dead, Attacking
    }
    public abstract class Zombie: Entity, IZombie
    {
        /// <summary>
        /// Behavior
        /// </summary>
        
        // 僵尸属性
        protected BindableProperty<float> healthPoint;
        protected Attack attack = new Attack {damage = 50};
        protected float moveSpeed = 1.5f;
        

        // 变量
        protected FSM<ZombieState> behaviorState = new();
        protected IDealAttack attackingTarget = null;
        
        # region 僵尸行为(方法)
        public virtual void DealAttack(Attack attack)
        {
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
            if (attack.damage !=0)
            {
                this.healthPoint.Value -= attack.damage;
            }
        }
        protected virtual void Dead()
        {
            DOTween.Kill(_Rigidbody2D);
            gameObject.DestroySelf();
        }
        protected virtual void Jump()
        {
            _Rigidbody2D.velocity = new Vector2(_Rigidbody2D.velocity.x,Global.zombieJumpSpeed);
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
                attack.damage *= 0.5f;
            }

            slowness = true;
            slowTime = 3f;
        }

        protected virtual void EndSlowness()
        {
            attack.damage *= 2f;
            moveSpeed *= 2f;
            slowness = false;
        }
        # endregion
        
        # region 僵尸行为(逻辑)

        protected virtual void SetUpState()
        {
            behaviorState ??= new FSM<ZombieState>();
            // Targeting
            behaviorState.State(ZombieState.Targeting)
                .OnEnter(() =>
                {
                    OnTriggerEnterEvent += AITargeting_FindPlant;
                })
                .OnUpdate(AITargeting_Targeting)
                .OnExit(() =>
                {
                    OnTriggerEnterEvent -= AITargeting_FindPlant;
                })
                ;
            // Attacking
            behaviorState.State(ZombieState.Attacking)
                .OnEnter(() =>
                {
                    this._Rigidbody2D.velocity = new Vector2(0, this._Rigidbody2D.velocity.y);
                    OnTriggerExitEvent += AIAttacking_WhenLoseTarget;
                }).OnUpdate(() =>
                {
                    attackingTarget?.DealAttack(attack.DamageMultiplier(Time.deltaTime));
                })
                .OnExit(() =>
                {
                    OnTriggerExitEvent -= AIAttacking_WhenLoseTarget;
                });
                
                // Dead
            behaviorState.State(ZombieState.Dead)
                .OnEnter(Dead);
            behaviorState.StartState(ZombieState.Targeting);
        }

        protected virtual void AITargeting_FindPlant(Collider2D other)
        // 检测植物, 如果检测到了就吃
        {
            if (other.CompareTag("Plant"))
            {
                this.behaviorState.ChangeState(ZombieState.Attacking);
                attackingTarget = other.GetComponent<Plant>() as IDealAttack;
            }
        }
        protected virtual void AITargeting_Targeting()
        // 寻路中(跟随戴夫)
        {
            // 走路
            if (_LevelModel.Dave.transform.position.x > transform.position.x)
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
        protected virtual void AIAttacking_WhenLoseTarget(Collider2D other) 
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

        
        // 初始化
        public void Initialize()
        {
            
        }
        /// <summary>
        /// Code
        /// </summary>
        
        // 引用
        protected Rigidbody2D _Rigidbody2D;
        
        // 初始化
        protected override void Awake()
        {
            base.Awake();
            //
            // 获取
            _Rigidbody2D = GetComponent<Rigidbody2D>();
            // 初始化数据
            healthPoint = new BindableProperty<float>(100);
            healthPoint.Register(val =>
            {
                if (val <= 0)
                {
                    behaviorState.ChangeState(ZombieState.Dead);
                }
            });
            SetUpState();
        }

        

    }
}