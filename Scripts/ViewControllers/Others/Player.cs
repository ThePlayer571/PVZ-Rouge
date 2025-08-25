using System;
using DG.Tweening;
using QFramework;
using TPL.PVZR.Classes.DataClasses_InLevel.Attack;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.CommandEvents._NotClassified_;
using TPL.PVZR.Helpers.New.Methods;
using TPL.PVZR.Models;
using TPL.PVZR.Services;
using TPL.PVZR.Tools;
using TPL.PVZR.ViewControllers.Entities.EntityBase.Interfaces;
using TPL.PVZR.ViewControllers.Entities.Plants.Base;
using TPL.PVZR.ViewControllers.Managers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TPL.PVZR.ViewControllers
{
    public interface IPlayer : IEntity, IAttackable
    {
    }

    public partial class Player
    {
        [SerializeField] private Transform _View;
        [SerializeField] private SpriteRenderer _SpriteRenderer;

        private IAudioService _AudioService;

        //todo step音效
        private FMOD.Studio.EventInstance _stepEventInstance;

        private EasyEvent OnJumped = new();

        private void OnViewInit()
        {
            _AudioService = this.GetService<IAudioService>();
            // _stepEventInstance = _AudioService.CreateEventInstance();

            _direction.Register(dir => { transform.LocalScaleX(dir.ToInt() * 0.7f); }
            ).UnRegisterWhenGameObjectDestroyed(this);

            hitCount.Register(val =>
            {
                var red = val == 0 ? 0 : Mathf.Lerp(0.1f, 1, val / 20f);
                _SpriteRenderer.DOColor(new Color(1, 1 - red, 1 - red, 1), 0.05f);
            }).UnRegisterWhenGameObjectDestroyed(this);

            _healthFSM.OnStateChanged((oldState, newState) =>
            {
                if (oldState == PlayerHealthState.Healthy)
                {
                    _View.DOLocalRotate(new Vector3(0, 0, 90), 0.2f).SetEase(Ease.Linear).SetId("DownedRotation");
                }

                if (newState == PlayerHealthState.Healthy)
                {
                    _View.DOLocalRotate(Vector3.zero, 0.2f).SetEase(Ease.Linear).SetId("DownedRotation");
                }
            });
        }

        private float _targetRotationZ = 0;
        private float _currentRotationZ = 0;
        private const float _rotationSpeed = 5f;

        private void OnViewUpdate()
        {
            // 倾斜
            if (!Mathf.Approximately(_movementInput.x, 0))
            {
                _targetRotationZ = -5f;
            }
            else
            {
                _targetRotationZ = 0f;
            }

            if (!Mathf.Approximately(_targetRotationZ, _currentRotationZ))
            {
                _currentRotationZ = Mathf.Lerp(_currentRotationZ, _targetRotationZ, Time.deltaTime * _rotationSpeed);
                if (_healthFSM.CurrentStateId == PlayerHealthState.Healthy && !DOTween.IsTweening("DownedRotation"))
                {
                    _View.LocalEulerAnglesZ(_currentRotationZ);
                }
            }

            // // 音效 - 走路
            // if (!Mathf.Approximately(_movementInput.x, 0))
            // {
            //     if (JumpDetector.HasTarget)
            //     {
            //     }
            // }
        }
    }

    public partial class Player : MonoBehaviour, IPlayer
    {
        #region 字段

        // 可配置项
        [SerializeField] private float speed = 30;
        [SerializeField] private float jumpForce = 9;
        [SerializeField] private float k = 5;
        [SerializeField] private float climbSpeed = 5;

        // 
        public static Player Instance { get; private set; }
        private PlayerInputControl _inputActions;
        private IPhaseModel _PhaseModel;
        private ILevelGridModel _LevelGridModel;

        [SerializeField] private TriggerDetector JumpDetector;
        [SerializeField] private TriggerDetector LadderDetector;

        private Rigidbody2D _Rigidbody2D;

        // 变量
        private bool _hasTwiceJumped = false; // 已经进行二段跳
        private BindableProperty<int> hitCount = new(0);
        private FSM<PlayerHealthState> _healthFSM;

        #endregion

        #region 生命周期

        private BindableProperty<Direction2> _direction = new(Direction2.Right);
        private Vector2 _movementInput;

        private void FixedUpdate()
        {
            // input
            if (_healthFSM.CurrentStateId == PlayerHealthState.Healthy)
            {
                _movementInput = _inputActions.Level.Movement.ReadValue<Vector2>();

                if (!Mathf.Approximately(_movementInput.x, 0))
                {
                    _direction.Value = _movementInput.x > 0 ? Direction2.Right : Direction2.Left;
                    _Rigidbody2D.AddForce(new Vector2(speed * _direction.Value.ToInt(), 0));
                }

                if (_movementInput.y > 0 && LadderDetector.HasTarget)
                {
                    ClimbLadder();
                }
            }

            // dragForce
            var dragForce = new Vector2(-k * _Rigidbody2D.velocity.x, 0);
            _Rigidbody2D.AddForce(dragForce);
        }

        private void Update()
        {
            if (_hasTwiceJumped && JumpDetector.HasTarget)
            {
                _hasTwiceJumped = false;
            }

            _healthFSM.Update();

            OnViewUpdate();
        }

        private void OnDestroy()
        {
            _inputActions.Level.Jump.performed -= TryJump;
        }


        private void TryJump(InputAction.CallbackContext _)
        {
            if (_healthFSM.CurrentStateId == PlayerHealthState.Healthy)
            {
                Jump();
            }
        }


        private void Awake()
        {
            Player.Instance = this;

            _PhaseModel = this.GetModel<IPhaseModel>();
            _LevelGridModel = this.GetModel<ILevelGridModel>();

            _Rigidbody2D = this.GetComponent<Rigidbody2D>();

            JumpDetector.TargetPredicate = (collider2D) =>
            {
                if (collider2D.IsInLayerMask(LayerMask.GetMask("PlantPlayerInteraction")) &&
                    collider2D.GetComponentInParent<Plant>().Def.Id is not (PlantId.Flowerpot or PlantId.LilyPad))
                    return false;
                return true;
            };

            SetUpHealthFSM();


            _inputActions = InputManager.Instance.InputActions;

            _inputActions.Level.Jump.performed += TryJump;

            OnViewInit();
        }

        #endregion

        #region 移动

        private void Jump()
        {
#if UNITY_EDITOR
            if (UnityEngine.Input.GetKey(KeyCode.LeftShift) || UnityEngine.Input.GetKey(KeyCode.RightShift))
            {
                // 按住shift键可以无限跳
                _Rigidbody2D.velocity = new Vector2(_Rigidbody2D.velocity.x, jumpForce);
                return;
            }
#endif

            if (JumpDetector.HasTarget)
            {
                _Rigidbody2D.velocity = new Vector2(_Rigidbody2D.velocity.x, jumpForce);
            }
            else if (!_hasTwiceJumped)
            {
                _hasTwiceJumped = true;
                _Rigidbody2D.velocity = new Vector2(_Rigidbody2D.velocity.x, jumpForce);
            }
            else // 已经二段跳，不能跳了
            {
                // do nothing
            }
        }

        private void ClimbLadder()
        {
            _Rigidbody2D.velocity = new Vector2(_Rigidbody2D.velocity.x, climbSpeed);
        }

        #endregion

        #region 击倒

        private void SetUpHealthFSM()
        {
            Timer hitCountClearTimer = new Timer(1);
            Timer invulnerableTimer = new Timer(0.5f);

            _healthFSM = new FSM<PlayerHealthState>();
            _healthFSM.State(PlayerHealthState.Healthy);
            _healthFSM.State(PlayerHealthState.Downed)
                .OnEnter(() => { hitCountClearTimer.SetRemaining(2); })
                .OnUpdate(() =>
                {
                    hitCountClearTimer.Update(Time.deltaTime);
                    if (hitCountClearTimer.Ready)
                    {
                        hitCount.Value--;
                        hitCountClearTimer.Reset();
                        if (hitCount.Value <= 0)
                        {
                            _healthFSM.ChangeState(PlayerHealthState.Healthy);
                        }
                    }
                });
            _healthFSM.State(PlayerHealthState.DownedInvulnerable)
                .OnEnter(() => { invulnerableTimer.Reset(); })
                .OnUpdate(() =>
                {
                    invulnerableTimer.Update(Time.deltaTime);
                    if (invulnerableTimer.Ready)
                    {
                        _healthFSM.ChangeState(PlayerHealthState.Downed);
                    }
                });
            _healthFSM.StartState(PlayerHealthState.Healthy);
        }

        #endregion

        #region 接口

        public IArchitecture GetArchitecture()
        {
            return PVZRouge.Interface;
        }

        public Vector2Int CellPos => LevelGridHelper.WorldToCell(this.transform.position);


        public AttackData TakeAttack(AttackData attackData)
        {
            if (GameManager.Instance.invulnerable) return null;

            switch (_healthFSM.CurrentStateId)
            {
                case PlayerHealthState.Downed:
                    hitCount.Value++;
                    _Rigidbody2D.AddForce(attackData.Punch(MassCenter.position), ForceMode2D.Impulse);
                    if (hitCount.Value >= 20)
                    {
                        if (_PhaseModel.GamePhase is GamePhase.ChooseSeeds or GamePhase.Gameplay)
                        {
                            this.SendCommand<OnKilledByZombieCommand>();
                        }
                    }

                    _healthFSM.ChangeState(PlayerHealthState.DownedInvulnerable);

                    break;
                case PlayerHealthState.Healthy:
                    hitCount.Value++;
                    _Rigidbody2D.AddForce(attackData.Punch(MassCenter.position), ForceMode2D.Impulse);
                    _healthFSM.ChangeState(PlayerHealthState.DownedInvulnerable);
                    break;
            }

            return null;
        }

        [SerializeField] private Transform MassCenter;

        public void Kill()
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    internal enum PlayerHealthState
    {
        Healthy,
        Downed,
        DownedInvulnerable,
    }
}