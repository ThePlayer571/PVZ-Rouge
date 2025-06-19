using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TPL.PVZR.ViewControllers
{
    public class Player : MonoBehaviour
    {
        private PlayerInputControl _inputActions;
        private Rigidbody2D _Rigidbody;

        [SerializeField] private float speed;
        [SerializeField] private float jumpForce;
        [SerializeField] private float k;

        private void FixedUpdate()
        {
            // input
            var movement = _inputActions.Level.Movement.ReadValue<Vector2>();
            if (!Mathf.Approximately(movement.x, 0))
            {
                _Rigidbody.AddForce(new Vector2(speed * movement.x, 0));
            }

            // dragForce
            var dragForce = new Vector2(-k * _Rigidbody.velocity.x, 0);
            _Rigidbody.AddForce(dragForce);
        }

        private void Awake()
        {
            _inputActions = new PlayerInputControl();
            _inputActions.Level.Enable();

            _inputActions.Level.Jump.performed += (context) =>
            {
                _Rigidbody.velocity = new Vector2(_Rigidbody.velocity.x, jumpForce);
            };
            _Rigidbody = this.GetComponent<Rigidbody2D>();
        }
    }
}