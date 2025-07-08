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
        [SerializeField] private float climbSpeed;

        [SerializeField] private CollisionDetector JumpDetector;
        [SerializeField] private CollisionDetector LadderDetector;

        private void FixedUpdate()
        {
            // input
            var movement = _inputActions.Level.Movement.ReadValue<Vector2>();
            if (!Mathf.Approximately(movement.x, 0))
            {
                var direction = movement.x > 0 ? 1 : -1;
                _Rigidbody2D.AddForce(new Vector2(speed * direction, 0));
            }

            if (movement.y > 0 && LadderDetector.HasTarget)
            {
                ClimbLadder();
            }

            // dragForce
            var dragForce = new Vector2(-k * _Rigidbody2D.velocity.x, 0);
            _Rigidbody2D.AddForce(dragForce);
        }


        private bool _hasTwiceJumped = false; // 已经进行二段跳

        private void Jump()
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

        private void ClimbLadder()
        {
            _Rigidbody2D.velocity = new Vector2(_Rigidbody2D.velocity.x, climbSpeed);
        }

        private void Update()
        {
            if (_hasTwiceJumped && JumpDetector.HasTarget)
            {
                _hasTwiceJumped = false;
            }
        }

        private void Awake()
        {
            _Rigidbody2D = this.GetComponent<Rigidbody2D>();
            _Collider = this.GetComponent<Collider2D>();

            JumpDetector.TargetPredicate = (collider2D) =>
            {
                if (collider2D.IsInLayerMask(LayerMask.GetMask("Barrier"))) return true;
                if (collider2D.IsInLayerMask(LayerMask.GetMask("Plant")) &&
                    collider2D.GetComponent<Flowerpot>() != null) return true;
                return false;
            };


            _inputActions = new PlayerInputControl();
            _inputActions.Level.Enable();

            _inputActions.Level.Jump.performed += (context) => { Jump(); };

            ReferenceHelper.Player = this;
        }


        #region 接口

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

        #endregion
    }
}