using System;
using QFramework;
using TPL.PVZR.Classes.DataClasses_InLevel.Attack;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.Helpers.New.Methods;
using TPL.PVZR.Tools;
using TPL.PVZR.ViewControllers.Entities.EntityBase.Interfaces;
using TPL.PVZR.ViewControllers.Entities.Plants.Base;
using UnityEngine;

namespace TPL.PVZR.ViewControllers
{
    public interface IPlayer : IEntity, IAttackable
    {
    }

    public class Player : MonoBehaviour, IPlayer
    {
        public static Player Instance { get; private set; }
        private PlayerInputControl _inputActions;
        private Rigidbody2D _Rigidbody2D;

        [SerializeField] private float speed = 30;
        [SerializeField] private float jumpForce = 9;
        [SerializeField] private float k = 5;
        [SerializeField] private float climbSpeed = 5;

        [SerializeField] private TriggerDetector JumpDetector;
        [SerializeField] private TriggerDetector LadderDetector;

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

        private void OnDestroy()
        {
            _inputActions.Level.Disable();
        }

        private void Awake()
        {
            Player.Instance = this;
            _Rigidbody2D = this.GetComponent<Rigidbody2D>();

            JumpDetector.TargetPredicate = (collider2D) =>
            {
                if (collider2D.IsInLayerMask(LayerMask.GetMask("PlantPlayerInteraction")) &&
                    collider2D.GetComponentInParent<Plant>().Def.Id is not (PlantId.Flowerpot or PlantId.LilyPad))
                    return false;
                return true;
            };


            _inputActions = new PlayerInputControl();
            _inputActions.Level.Enable();

            _inputActions.Level.Jump.performed += (context) => { Jump(); };
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

        #endregion
    }
}