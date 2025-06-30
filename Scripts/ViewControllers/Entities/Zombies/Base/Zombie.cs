using System;
using QFramework;
using TPL.PVZR.Classes;
using TPL.PVZR.Classes.DataClasses.Attack;
using TPL.PVZR.Classes.DataClasses.Effect;
using TPL.PVZR.Classes.ZombieAI.Public;
using TPL.PVZR.CommandEvents._NotClassified_;
using TPL.PVZR.Helpers.Methods;
using TPL.PVZR.Systems;
using TPL.PVZR.Tools;
using TPL.PVZR.ViewControllers.Entities.EntityBase;
using TPL.PVZR.ViewControllers.Entities.EntityBase.Interfaces;
using TPL.PVZR.ViewControllers.Entities.Zombies.States;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace TPL.PVZR.ViewControllers.Entities.Zombies.Base
{
    public abstract class Zombie : Entity, IZombie
    {
        #region 调试

        [SerializeField] public bool triggerDebug = false;

        #endregion

        #region 生命周期函数

        protected override void Awake()
        {
            base.Awake();

            _ZombieAISystem = this.GetSystem<IZombieAISystem>();


            // 血量
            Health = new BindableProperty<float>();
            // 移动
            Direction = new BindableProperty<Direction2>();
            // 跳跃
            _jumpTimer = new Timer(Global.Zombie_Default_JumpInterval);

            effectGroup = new EffectGroup();

            // AI / 行为主控
            _FSM = new FSM<ZombieState>();
            this.RegisterEvent<OnPlayerChangeCluster>(e => _timeToFindPath = true)
                .UnRegisterWhenGameObjectDestroyed(this);
        }

        protected override void Update()
        {
            base.Update();
            //
            _FSM.Update();
            _jumpTimer.Update(Time.deltaTime);
            effectGroup.Update(Time.deltaTime);
        }

        private void FixedUpdate()
        {
            // 水平方向的拉力
            var dragForce = new Vector2(-5 * _Rigidbody2D.velocity.x, 0);
            _Rigidbody2D.AddForce(dragForce);
        }

        #endregion

        #region AI / 行为主控

        private FSM<ZombieState> _FSM;
        public IAttackable AttackingTarget;

        protected virtual void SetUpFSM()
        {
            _FSM.AddState(ZombieState.DefaultTargeting, new DefaultTargetingState(_FSM, this));
            _FSM.AddState(ZombieState.Attacking, new AttackingState(_FSM, this));

            _FSM.StartState(ZombieState.DefaultTargeting);
        }

        /// <summary>
        /// 定义：初始化，能用于对象池的初始化方法
        /// </summary>
        public virtual void Initialize()
        {
            SetUpFSM();
        }

        #endregion

        #region 属性

        // Designer
        [SerializeField] public ZombieAttackAreaController AttackArea;
        [SerializeField] public Transform JumpDetectionPoint;
        public IZombieAISystem _ZombieAISystem;

        // 基础属性
        public abstract ZombieId Id { get; }

        public float baseSpeed = 2f;
        public float baseJumpForce = 5f;
        public AttackData baseAttackData = null;

        #region 当前属性（考虑Effect后）

        public AttackData CreateAttackData()
        {
            var attackData = new AttackData(baseAttackData);

            foreach (var effectData in effectGroup)
            {
                switch (effectData.effectId)
                {
                    case EffectId.Chill:
                        attackData.MultiplyDamage(EffectHelper.Zombie_Chill_AttackFactor(effectData.level));
                        break;
                    default:
                        break;
                }
            }

            return attackData;
        }

        public float GetSpeed()
        {
            var speed = baseSpeed;
            foreach (var effectData in effectGroup)
            {
                switch (effectData.effectId)
                {
                    case EffectId.Chill:
                        speed *= EffectHelper.Zombie_Chill_SpeedFactor(effectData.level);
                        break;
                    default:
                        break;
                }
            }

            return speed;
        }

        public float GetJumpForce()
        {
            return baseJumpForce;
        }

        #endregion

        // 变量
        public BindableProperty<Direction2> Direction;
        public BindableProperty<float> Health;

        #endregion

        #region Effect

        protected EffectGroup effectGroup;


        public void GiveEffect(EffectData effectData)
        {
            effectGroup.GiveEffect(effectData);
        }

        #endregion

        #region 血量

        public AttackData TakeAttack(AttackData attackData)
        {
            if (attackData == null) return null;
            Health.Value = Mathf.Clamp(Health.Value - attackData.Damage, 0, Mathf.Infinity);
            _Rigidbody2D.AddForce(attackData.Punch(_Rigidbody2D.position), ForceMode2D.Impulse);
            foreach (var effectData in attackData.Effects)
            {
                this.GiveEffect(effectData);
            }

            return null;
        }

        #endregion

        #region 攻击

        #endregion

        #region 移动

        public bool _timeToFindPath = false;
        public AITendency AITendency = AITendency.Default;
        public IZombiePath CachePath = null;
        [SerializeField, ReadOnly] public MoveData CurrentMoveData = null;


        [Unsafe("调用此方法前，请确保路径百分百能被找到，不然会出错误")]
        public void FindPath(Vector2Int targetPos)
        {
            CachePath = _ZombieAISystem.ZombieAIUnit.FindPath(CellPos, targetPos, AITendency);
            CurrentMoveData = CachePath.NextTarget();
        }

        public virtual void MoveTowards(MoveData moveData)
        {
            switch (moveData.moveType)
            {
                case MoveType.WalkJump:
                {
                    Vector2 targetPos;
                    switch (moveData.moveStage)
                    {
                        case MoveStage.FollowVertex:
                            targetPos = moveData.targetWorldPos;
                            break;
                        case MoveStage.FindDave:
                            targetPos = ReferenceHelper.Player.transform.position;
                            break;
                        default: throw new NotImplementedException();
                    }

                    var hit = Physics2D.Raycast(JumpDetectionPoint.position, Direction.Value.ToVector2(), 0.5f,
                        LayerMask.GetMask("Barrier"));
                    // ====== 临时调试代码 begin ======
#if UNITY_EDITOR
                    Color color = hit.collider ? Color.red : Color.green;
                    Debug.DrawLine(JumpDetectionPoint.position,
                        JumpDetectionPoint.position + (Vector3)Direction.Value.ToVector2() * 0.5f,
                        color, 0.1f);
                    if (hit.collider)
                    {
                        Debug.DrawLine(JumpDetectionPoint.position, hit.point, Color.yellow, 0.1f);
                    }
#endif
                    // ====== 临时调试代码 end ======
                    if (hit.collider) TryJump();

                    float distance = Mathf.Abs(transform.position.x - targetPos.x);
                    if (distance < 0.1f) return;


                    this.Direction.Value = (transform.position.x > targetPos.x)
                        ? Direction2.Left
                        : Direction2.Right;
                    _Rigidbody2D.AddForce(Direction.Value.ToVector2() * this.GetSpeed());
                    break;
                }
                case MoveType.Fall:
                {
                    float distance = Mathf.Abs(transform.position.x - CurrentMoveData.targetWorldPos.x);
                    if (distance < 0.1f) return;


                    this.Direction.Value = (transform.position.x > CurrentMoveData.targetWorldPos.x)
                        ? Direction2.Left
                        : Direction2.Right;
                    _Rigidbody2D.AddForce(Direction.Value.ToVector2() * this.GetSpeed());
                    break;
                }

                default: return;
            }
        }

        #endregion

        #region 跳跃

        public Timer _jumpTimer { get; set; }


        public void Jump()
        {
            _Rigidbody2D.AddForce(Vector2.up * this.GetJumpForce(), ForceMode2D.Impulse);
            _jumpTimer.Reset();
        }

        public void TryJump()
        {
            if (!_jumpTimer.Ready) return;
            Jump();
        }

        #endregion
    }
}