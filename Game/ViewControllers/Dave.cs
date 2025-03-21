using UnityEngine;
using QFramework;
using System;
using DG.Tweening;

namespace TPL.PVZR
{
    public partial class Dave : ViewController, IController, IDealAttack
    {
        # region 不重要的


        public enum JumpState
        {
            NotTwiceJumped,
            TwiceJumped
        }

        public enum DaveState
        {
            Normal,
            KnockedDown,
            Dead
        }
        // 框架接口
        public IArchitecture GetArchitecture()
        {
            return PVZRouge.Interface;
        }

        #endregion

        # region MainLogic

        // Model|System
        private IDaveModel _DaveModel;

        // 引用
        private Rigidbody2D _Rigidbody2D;
        private Collider2D _Collider2D;

        private SpriteRenderer _SpriteRenderer;

        // == 逻辑
        private void Update()
        {
            _jumpState.Update();
            // DavaState
            _daveState.Update();
            if (_stunTimer > 0)
            {
                _stunTimer -= Time.deltaTime;
            }
        }

        private void Awake()
        {
            _DaveModel = this.GetModel<IDaveModel>();
            //
            _Rigidbody2D = GetComponent<Rigidbody2D>();
            _Collider2D = GetComponent<Collider2D>();
            _SpriteRenderer = GetComponent<SpriteRenderer>();
            // DaveState
            _health.RegisterWithInitValue(val =>
                    _SpriteRenderer.DOBlendableColor(Color.HSVToRGB(0, Mathf.Lerp(1, 0, _health.Value / 100f), 1), 0f))
                .UnRegisterWhenGameObjectDestroyed(this);
            SetUpStateDave();
            // Jump
            SetUpStateJump();
            this.RegisterEvent<InputJumpEvent>(TryJump).UnRegisterWhenGameObjectDestroyed(this);
            // Move
            this.RegisterEvent<InputMoveEvent>(TryMove).UnRegisterWhenGameObjectDestroyed(this);
        }

        #endregion

        # region Behavior: Jump

        // 变量
        private FSM<JumpState> _jumpState = new();

        // 属性
        private bool isOnGround =>
            Physics2D.OverlapArea(jumpDetectRegionMin, jumpDetectRegionMax, LayerMask.GetMask("Barrier"));

        private Vector3 jumpDetectRegionMin => _Collider2D.bounds.min + new Vector3(0.1f, -0.1f); // 左下

        private Vector3 jumpDetectRegionMax =>
            _Collider2D.bounds.min + new Vector3(_Collider2D.bounds.extents.x - 0.1f, 0); // 右上

        // 操作
        private void TryJump(InputJumpEvent inputJumpEvent)
        {
            // 戴夫仅在Normal时能跳
            if (_daveState.CurrentStateId != DaveState.Normal) return;
            // 二段跳逻辑
            // 所有情况：(1)在地上 (2)在空中且未进行二段跳 (3)在空中且进行过二段跳
            if (isOnGround) // Case (1)
            {
                Jump();
            }
            else if (_jumpState.CurrentStateId == JumpState.NotTwiceJumped) // Case (2)
            {
                Jump();
                _jumpState.ChangeState(JumpState.TwiceJumped);
            }
            // Case (3): Do Nothing

        }

        private void Jump()
        {
            _Rigidbody2D.velocity = new Vector2(_Rigidbody2D.velocity.x, _DaveModel.jumpSpeed);
        }

        private void SetUpStateJump()
        {

            _jumpState.State(JumpState.NotTwiceJumped);
            _jumpState.State(JumpState.TwiceJumped)
                .OnUpdate(() =>
                {
                    if (isOnGround)
                    {
                        _jumpState.ChangeState(JumpState.NotTwiceJumped);
                    }
                })
                ;
            _jumpState.StartState(JumpState.NotTwiceJumped);
        }

        #endregion

        # region Behavior: Health

        // 变量
        private FSM<DaveState> _daveState = new();
        private bool _isBeingEaten => _stunTimer > 0;
        private float _stunTimer = 0;

        // IDealAttack
        private BindableProperty<float> _health = new(100f);

        public void Kill()
        {
            throw new NotImplementedException();
        }

        public void DealAttack(Attack attack)
        {
            if (_daveState.CurrentStateId == DaveState.Dead) return;
            if (attack.isFrameDamage)
            {
                _health.Value = Mathf.Clamp(_health.Value - attack.damage * Time.deltaTime, 0, 100);
            }
            else
            {
                _health.Value = Mathf.Clamp(_health.Value - attack.damage, 0, 100);
            }

            _stunTimer = 1f;
        }

        private void SetUpStateDave()
        {
            _daveState.State(DaveState.Normal)
                .OnUpdate(() =>
                {
                    if (_health.Value < 100)
                    {
                        _daveState.ChangeState(DaveState.KnockedDown);
                    }
                });
            _daveState.State(DaveState.KnockedDown)
                .OnUpdate(() =>
                {
                    // 恢复血量
                    if (!_isBeingEaten)
                    {
                        _health.Value = Mathf.Clamp(_health.Value + Time.deltaTime, 0, 100);
                    }

                    // 状态切换
                    if (_health.Value >= 100)
                    {
                        _daveState.ChangeState(DaveState.Normal);
                    }

                    if (_health.Value <= 0)
                    {
                        _daveState.ChangeState(DaveState.Dead);
                    }
                });
            _daveState.State(DaveState.Dead)
                .OnEnter(() =>
                {
                    _health.Value = 0;
                    this.GetSystem<ILevelSystem>().levelState.ChangeState(LevelSystem.LevelState.EndGameplay);
                    this.GetSystem<ILevelSystem>().levelState.ChangeState(LevelSystem.LevelState.Defeat);
                });
            _daveState.StartState(DaveState.Normal);
        }

        #endregion

        #region Behavior: Move

        private float _currentMovementInputX = 0;
        private void TryMove(InputMoveEvent inputMoveEvent)
        {
            if (_daveState.CurrentStateId != DaveState.Normal) return;
            Move(inputMoveEvent);
        }

        private void Move(InputMoveEvent inputMoveEvent)
        {
            _currentMovementInputX = inputMoveEvent.speed;
        }

        private void FixedUpdate()
        {
            var newVelocity = new Vector2(0,_Rigidbody2D.velocity.y);
            // Case(1): 正在被吃
            if (_daveState.CurrentStateId != DaveState.Normal)
            {
                _Rigidbody2D.velocity = newVelocity;
                return;
            }
            // Case(2): 正常状态
            newVelocity.x += _currentMovementInputX *6f;
            _Rigidbody2D.velocity = newVelocity;
            
        }

        #endregion
    }
}
