using System;
using QFramework;
using TPL.PVZR.Classes.DataClasses.Attack;
using TPL.PVZR.Helpers.Methods;
using TPL.PVZR.Tools;
using TPL.PVZR.ViewControllers.Entities;
using TPL.PVZR.ViewControllers.Entities.EntityBase.Interfaces;
using TPL.PVZR.ViewControllers.Entities.Plants;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TPL.PVZR.ViewControllers
{
    public interface IPlayer : IEntity, IAttackable
    {
    }

    public class Player : MonoBehaviour, IPlayer

    {
        private PlayerInputControl _inputActions;
        private Rigidbody2D _Rigidbody2D;
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
                _Rigidbody2D.AddForce(new Vector2(speed * movement.x, 0));
            }

            // dragForce
            var dragForce = new Vector2(-k * _Rigidbody2D.velocity.x, 0);
            _Rigidbody2D.AddForce(dragForce);
        }


        private bool _hasTwiceJumped = false; // 已经进行二段跳

        private void Jump()
        {
            var start = new Vector2(this.transform.position.x, _Collider.bounds.min.y);
            var size = new Vector2(0.5f, 0.02f);
            bool OnGround = Physics2D.OverlapBox(start, size, 0, LayerMask.GetMask("Barrier"));

            if (!OnGround)
            {
                // 检查Plant层是否有Flowerpot
                var colliders = Physics2D.OverlapBoxAll(start, size, 0, LayerMask.GetMask("Plant"));
                foreach (var col in colliders)
                {
                    if (col.GetComponent<Flowerpot>() != null)
                    {
                        OnGround = true;
                        break;
                    }
                }
            }

            if (OnGround)
            {
                _hasTwiceJumped = false;
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

        private void Awake()
        {
            _Rigidbody2D = this.GetComponent<Rigidbody2D>();
            _Collider = this.GetComponent<Collider2D>();

            _inputActions = new PlayerInputControl();
            _inputActions.Level.Enable();

            _inputActions.Level.Jump.performed += (context) => { Jump(); };

            ReferenceHelper.Player = this;
        }

        public IArchitecture GetArchitecture()
        {
            return PVZRouge.Interface;
        }

        public Vector2Int CellPos => LevelGridHelper.WorldToCell(this.transform.position);


        public AttackData TakeAttack(AttackData attackData)
        {
            return null;
        }

        public void Kill()
        {
            throw new NotImplementedException();
        }

        public void Remove()
        {
            throw new NotImplementedException();
        }

        public void Dead()
        {
            throw new NotImplementedException();
        }
    }
}