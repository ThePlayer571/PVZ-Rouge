using System;
using System.Collections.Generic;
using QFramework;
using TPL.PVZR.Classes;
using TPL.PVZR.Classes.DataClasses_InLevel.Attack;
using TPL.PVZR.Classes.DataClasses_InLevel.Effect;
using TPL.PVZR.Classes.DataClasses_InLevel.ZombieArmor;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.Classes.ZombieAI.Public;
using TPL.PVZR.CommandEvents.__NewlyAdded__;
using TPL.PVZR.CommandEvents._NotClassified_;
using TPL.PVZR.Helpers.New.Methods;
using TPL.PVZR.Systems;
using TPL.PVZR.Systems.Level_Data;
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
            FSM = new FSM<ZombieState>();
        }

        protected override void Update()
        {
            base.Update();
            //
            FSM.Update();
            _jumpTimer.Update(Time.deltaTime);
            effectGroup.Update(Time.deltaTime);
        }

        private void FixedUpdate()
        {
            // 水平方向的拉力
            var dragForce = new Vector2(-10 * _Rigidbody2D.velocity.x, 0);
            _Rigidbody2D.AddForce(dragForce);
        }

        #endregion

        #region AI / 行为主控

        public FSM<ZombieState> FSM;
        public IAttackable AttackingTarget;

        protected virtual void SetUpFSM()
        {
            FSM.AddState(ZombieState.DefaultTargeting, new DefaultTargetingState(FSM, this));
            FSM.AddState(ZombieState.Attacking, new AttackingState(FSM, this));

            FSM.StartState(ZombieState.DefaultTargeting);
        }

        public void Initialize()
        {
            SetUpFSM();
            // AI
            this.RegisterEvent<OnPlayerChangeCluster>(e => _timeToFindPath = true)
                .UnRegisterWhenGameObjectDestroyed(this);

            ActionKit.DelayFrame(1, OnInitialized.Trigger).Start(this);
            // 朝向
            this.Direction
                .RegisterWithInitValue(direction => { transform.LocalScaleX(direction.ToInt()); })
                .UnRegisterWhenGameObjectDestroyed(this);
            //
            OnInit();
        }

        #endregion

        #region 字段

        // Designer
        [SerializeField] public ZombieAttackAreaController AttackArea;
        [SerializeField] public Transform JumpDetectionPoint;
        [SerializeField] public CollisionDetector ClimbDetector;
        [SerializeField] public Transform MassCenter;
        [SerializeField] public Transform CorePos;
        public IZombieAISystem _ZombieAISystem;

        public override Vector2 CoreWorldPos => CorePos.position;

        // 基础属性
        public abstract ZombieId Id { get; }

        [NonSerialized] public float baseSpeed = 1.2f;
        [NonSerialized] public float baseJumpForce = 5f;
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

        // 事件
        public EasyEvent<AttackData> OnDieFrom = new();
        public EasyEvent OnInitialized = new();

        #endregion

        #region Effect

        protected EffectGroup effectGroup;


        public void GiveEffect(EffectData effectData)
        {
            effectGroup.GiveEffect(effectData);
        }

        #endregion

        #region 被攻击

        public virtual AttackData TakeAttack(AttackData attackData)
        {
            if (attackData == null) return null;
            Health.Value = Mathf.Clamp(Health.Value - attackData.Damage, 0, Mathf.Infinity);
            // !!!! 如果出现bug（力的生成和实际位置不一致）请看这里：质心不是Rigidbody2D
            _Rigidbody2D.AddForce(attackData.Punch(MassCenter.position), ForceMode2D.Impulse);
            foreach (var effectData in attackData.Effects)
            {
                this.GiveEffect(effectData);
            }

            if (Health.Value <= 0) DieWith(attackData);

            return null;
        }

        #endregion

        #region 实体生命周期

        public abstract void OnInit();

        public override void DieWith(AttackData attackData)
        {
            OnDieFrom.Trigger(attackData);
            foreach (var armorData in ZombieArmorList)
            {
                armorData.OnDestroyed.Trigger();
            }

            this.SendCommand<OnZombieDeathCommand>(new OnZombieDeathCommand(this));
        }

        public override void Remove()
        {
            throw new Exception("僵尸的移除应该交给ZombieFactory");
        }

        #endregion

        #region 移动

        public bool _timeToFindPath = false;
        public AITendency AITendency = AITendency.Default;
        public IZombiePath CachePath = null;
        [SerializeField, ReadOnly] public MoveData CurrentMoveData = null;


        [Unsafe("调用此方法前，请确保路径百分百能被找到，不然会出错误。安全的调用方法：_timeToFindPath = true，然后让FSM自动响应")]
        public void FindPath(Vector2Int targetPos)
        {
            CachePath = _ZombieAISystem.ZombieAIUnit.FindPath(CellPos, targetPos, AITendency);
            CurrentMoveData = CachePath.NextTarget();
        }

        public void MoveForward()
        {
            _Rigidbody2D.AddForce(Direction.Value.ToVector2() * this.GetSpeed());
        }

        public void ClimbLadder()
        {
            if (ClimbDetector.HasTarget)
            {
                _Rigidbody2D.velocity = new Vector2(_Rigidbody2D.velocity.x,
                    GlobalEntityData.Zombie_Default_ClimbSpeed);
            }
        }

        public void HoldOnLadder()
        {
            if (ClimbDetector.HasTarget)
            {
                _Rigidbody2D.velocity = new Vector2(_Rigidbody2D.velocity.x, 0);
            }
        }

        public virtual void MoveTowards(MoveData moveData)
        {
            switch (moveData.moveType)
            {
                case MoveType.WalkJump:
                {
                    Vector2 targetPos = moveData.moveStage switch
                    {
                        MoveStage.FollowVertex => moveData.targetWorldPos,
                        MoveStage.FindDave => ReferenceHelper.Player.transform.position,
                        _ => throw new ArgumentException($"不支持的MoveStage: {moveData.moveStage}")
                    };

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
                    if (distance < Global.Zombie_Default_PathFindStopMinDistance) return;


                    this.Direction.Value = (transform.position.x > targetPos.x)
                        ? Direction2.Left
                        : Direction2.Right;
                    MoveForward();
                    break;
                }
                case MoveType.Fall:
                {
                    float distance = Mathf.Abs(transform.position.x - CurrentMoveData.targetWorldPos.x);
                    if (distance < Global.Zombie_Default_PathFindStopMinDistance) return;


                    this.Direction.Value = (transform.position.x > CurrentMoveData.targetWorldPos.x)
                        ? Direction2.Left
                        : Direction2.Right;
                    MoveForward();
                    break;
                }
                case MoveType.ClimbLadder:
                {
                    Vector2 targetPos = moveData.moveStage switch
                    {
                        MoveStage.FollowVertex => moveData.targetWorldPos,
                        MoveStage.FindDave => ReferenceHelper.Player.transform.position,
                        _ => throw new ArgumentException($"不支持的MoveStage: {moveData.moveStage}")
                    };
                    float distanceX = Mathf.Abs(transform.position.x - targetPos.x);
                    float distanceY = Mathf.Abs(transform.position.y - targetPos.y);
                    if (distanceX > Global.Zombie_Default_PathFindStopMinDistance)
                    {
                        this.Direction.Value = (transform.position.x > targetPos.x)
                            ? Direction2.Left
                            : Direction2.Right;
                        MoveForward();
                    }

                    if (distanceY > Global.Zombie_Default_PathFindStopMinDistance)
                    {
                        bool toUp = transform.position.y < targetPos.y;
                        if (toUp)
                        {
                            ClimbLadder();
                        }
                    }

                    break;
                }
                case MoveType.ClimbWalkJump:
                {
                    Vector2 targetPos = moveData.moveStage switch
                    {
                        MoveStage.FollowVertex => moveData.targetWorldPos,
                        // MoveStage.FindDave => ReferenceHelper.Player.transform.position, // 禁用
                        _ => throw new ArgumentException($"不支持的MoveStage: {moveData.moveStage}")
                    };

                    float distanceX = Mathf.Abs(transform.position.x - targetPos.x);
                    float distanceY = Mathf.Abs(transform.position.y - targetPos.y);
                    if (distanceX > Global.Zombie_Default_PathFindStopMinDistance)
                    {
                        this.Direction.Value = (transform.position.x > targetPos.x)
                            ? Direction2.Left
                            : Direction2.Right;
                        MoveForward();
                    }

                    if (distanceY > Global.Zombie_Default_PathFindStopMinDistance)
                    {
                        bool toUp = transform.position.y < targetPos.y;
                        if (toUp && ClimbDetector.HasTarget)
                        {
                            ClimbLadder();
                        }
                    }

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

        #region 盔甲

        /// <summary>
        /// 存在盔甲则必须放到这里面，用于与外界数据交换
        /// </summary>
        public readonly List<ZombieArmorData> ZombieArmorList = new List<ZombieArmorData>();

        #endregion

        #region SortingLayer(Temp)

        //TODO 这是僵尸SortingLayer的临时解决方案，等其他模块有类似需求再改为统一的

        private static int sortingLayer { get; set; } = 0;

        public static int AllocateSortingLayer()
        {
            sortingLayer += 10;
            if (sortingLayer > 30000) sortingLayer = 0;
            return sortingLayer;
        }

        #endregion
    }
}