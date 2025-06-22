using System;
using TPL.PVZR.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TPL.PVZR.ViewControllers
{
    public class Player : MonoBehaviour
    {
        private PlayerInputControl _inputActions;
        private Rigidbody2D _Rigidbody;
        private Collider2D _Collider;

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


        private bool _hasTwiceJumped = false; // 已经进行二段跳

        private void Jump()
        {
            var start = new Vector2(this.transform.position.x, _Collider.bounds.min.y);
            var size = new Vector2(0.5f, 0.02f);
            bool OnGround = Physics2D.OverlapBox(start, size, 0, LayerMask.GetMask("Barrier"));
            
            if (OnGround)
            {
                _hasTwiceJumped = false;
                _Rigidbody.velocity = new Vector2(_Rigidbody.velocity.x, jumpForce);
            }
            else if (!_hasTwiceJumped)
            {
                _hasTwiceJumped = true;
                _Rigidbody.velocity = new Vector2(_Rigidbody.velocity.x, jumpForce);
            }
            else // 已经二段跳，不能跳了
            {
                // do nothing
            }
        }

        private void Awake()
        {
            _Rigidbody = this.GetComponent<Rigidbody2D>();
            _Collider = this.GetComponent<Collider2D>();

            _inputActions = new PlayerInputControl();
            _inputActions.Level.Enable();

            _inputActions.Level.Jump.performed += (context) => { Jump(); };

            ReferenceHelper.Player = this;
        }
    }
}