using QFramework;
using TPL.PVZR.Tools;
using TPL.PVZR.ViewControllers.Managers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TPL.PVZR.ViewControllers.Others
{
    public partial class PlayerInMainMenu
    {
        [SerializeField] private Transform _View;

        private void OnViewInit()
        {
            _direction.Register(dir => { transform.LocalScaleX(dir.ToInt() * 0.7f); }
            ).UnRegisterWhenGameObjectDestroyed(this);
        }

        private float _targetRotationZ = 0;
        private float _currentRotationZ = 0;
        private const float _rotationSpeed = 5f;

        private void OnViewUpdate()
        {
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
                _View.LocalEulerAnglesZ(_currentRotationZ);
            }
        }
    }

    public partial class PlayerInMainMenu : MonoBehaviour
    {
        // 可配置项
        private const float speed = 30;
        private const float jumpForce = 9;
        private const float k = 5;


        [SerializeField] private TriggerDetector JumpDetector;
        private Rigidbody2D _Rigidbody2D;

        private bool _hasTwiceJumped = false; // 已经进行二段跳
        private BindableProperty<Direction2> _direction = new(Direction2.Right);
        private Vector2 _movementInput;

        private void FixedUpdate()
        {
            // input
            _movementInput = InputManager.Instance.InputActions.MainMenu.Movement.ReadValue<Vector2>();

            if (!Mathf.Approximately(_movementInput.x, 0))
            {
                _direction.Value = _movementInput.x > 0 ? Direction2.Right : Direction2.Left;
                _Rigidbody2D.AddForce(new Vector2(speed * _direction.Value.ToInt(), 0));
            }

            // dragForce
            var dragForce = new Vector2(-k * _Rigidbody2D.velocity.x, 0);
            _Rigidbody2D.AddForce(dragForce);
        }

        private void Awake()
        {
            _Rigidbody2D = this.GetComponent<Rigidbody2D>();

            InputManager.Instance.InputActions.MainMenu.Jump.performed += Jump;
            OnViewInit();
        }

        private void Update()
        {
            if (_hasTwiceJumped && JumpDetector.HasTarget)
            {
                _hasTwiceJumped = false;
            }

            OnViewUpdate();
        }

        private void OnDestroy()
        {
            InputManager.Instance.InputActions.MainMenu.Jump.performed -= Jump;
        }

        private void Jump(InputAction.CallbackContext _)
        {
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
    }
}