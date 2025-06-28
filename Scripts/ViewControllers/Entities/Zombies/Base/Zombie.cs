using System;
using QFramework;
using TPL.PVZR.Classes;
using TPL.PVZR.Classes.LevelStuff;
using TPL.PVZR.Core;
using TPL.PVZR.Gameplay.Class.ZombieAI.Public;
using TPL.PVZR.Systems;
using TPL.PVZR.ViewControllers.Entities.Zombies.States;
using Unity.Collections;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Entities.Zombies
{
    public interface IZombie : IEntity, IEffectable, IAttackable
    {
    }

    public abstract class Zombie : Entity, IZombie
    {
        #region 调试

        [SerializeField] public bool triggerDebug = false;

        #endregion

        #region AI / 行为主控

        public IAttackable AttackingTarget { get; set; }

        protected override void Awake()
        {
            base.Awake();

            _ZombieAISystem = this.GetSystem<IZombieAISystem>();

            Direction = new BindableProperty<Direction2>();

            // Jump
            _jumpTimer = new Timer(jumpInterval);

            // FSM
            _FSM = new FSM<ZombieState>();
            _FSM.AddState(ZombieState.DefaultTargeting, new DefaultTargetingState(_FSM, this));
            _FSM.AddState(ZombieState.Attacking, new AttackingState(_FSM, this));

            _FSM.StartState(ZombieState.DefaultTargeting);
        }

        protected override void Update()
        {
            base.Update();
            //
            _FSM.Update();
            _jumpTimer.Update(Time.deltaTime);
        }

        private void FixedUpdate()
        {
            // dragForce
            var dragForce = new Vector2(-5 * _Rigidbody2D.velocity.x, 0);
            _Rigidbody2D.AddForce(dragForce);
        }

        private FSM<ZombieState> _FSM;

        #endregion

        #region 属性

        [SerializeField] public ZombieAttackAreaController AttackArea;
        [SerializeField] public Transform JumpDetectionPoint;

        protected float speed = 2f;
        protected float jumpForce = 5f;
        protected float jumpInterval = 1f;
        public BindableProperty<Direction2> Direction;

        #endregion

        #region Effect

        protected EffectGroup effectGroup;


        public void TakeEffect(Effect effect)
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #region 血量

        protected float health;

        public AttackData TakeAttack(AttackData attackData)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region 攻击

        private AttackData BasicAttackData;

        public AttackData CreateCurrentAttackData()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region 移动

        public AITendency AITendency { get; set; } = AITendency.Default;
        public IZombiePath CachePath { get; set; } = null;
        [SerializeField, ReadOnly] public MoveData CurrentMoveData = null;

        public void MoveTowards(MoveData moveData)
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
                    _Rigidbody2D.AddForce(Direction.Value.ToVector2() * speed);
                    break;
                }
                case MoveType.Fall:
                {
                    float distance = Mathf.Abs(transform.position.x - CurrentMoveData.targetWorldPos.x);
                    if (distance < 0.1f) return;


                    this.Direction.Value = (transform.position.x > CurrentMoveData.targetWorldPos.x)
                        ? Direction2.Left
                        : Direction2.Right;
                    _Rigidbody2D.AddForce(Direction.Value.ToVector2() * speed);
                    break;
                }

                default: throw new NotImplementedException();
            }
        }

        #endregion

        #region 跳跃

        public Timer _jumpTimer { get; set; }


        public void Jump()
        {
            _Rigidbody2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            _jumpTimer.Reset();
        }

        public void TryJump()
        {
            if (!_jumpTimer.Ready) return;
            Jump();
        }

        #endregion

        #region 引用

        public IZombieAISystem _ZombieAISystem { get; private set; }

        #endregion
    }
}